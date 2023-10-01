from fastapi import FastAPI,  File, Form, UploadFile, HTTPException
from pyhanko.pdf_utils.incremental_writer import IncrementalPdfFileWriter

from typing import Annotated
from uuid import UUID

from pyhanko.sign import signers, timestamps
from pyhanko.sign.fields import SigSeedSubFilter
from pyhanko_certvalidator import ValidationContext
import requests as req

from dto import *
import pyhanko.keys
from pyhanko.keys import load_private_key_from_pemder_data
from pyhanko.sign.validation import validate_pdf_signature
from pyhanko.pdf_utils.reader import PdfFileReader
from pyhanko_certvalidator import ValidationContext

from cryptography import x509
import cryptography as crypto

from pprint import pprint
import os

KEYSTORE = os.getenv("KEYSTORE_LOCATION")

app = FastAPI()

def get_certificate() -> x509.Certificate:
    pem_data = b"""
-----BEGIN CERTIFICATE-----
MIIDzjCCAragAwIBAgIUTmlOa7+RrcPPO107H9VJUS+FVQYwDQYJKoZIhvcNAQEL
BQAwbDELMAkGA1UEBhMCUEwxEzARBgNVBAgMCk1hbG9wb2xza2ExDzANBgNVBAcM
BkNyYWNvdzESMBAGA1UECgwJc3RoLWZpc2h5MQowCAYDVQQLDAFFMRcwFQYDVQQD
DA5zZXJ2ZXIuZXhhbXBsZTAeFw0yMzA5MzAyMzIwMjNaFw0yNDA5MjkyMzIwMjNa
MGwxCzAJBgNVBAYTAlBMMRMwEQYDVQQIDApNYWxvcG9sc2thMQ8wDQYDVQQHDAZD
cmFjb3cxEjAQBgNVBAoMCXN0aC1maXNoeTEKMAgGA1UECwwBRTEXMBUGA1UEAwwO
c2VydmVyLmV4YW1wbGUwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDC
yYtLOBd+ZcpcR0YtFvt9x3dfqplIjg2+e5cmGck+UnU44QhUZhctz7ydb/fo4IYa
4MzpPlerM6iYTx/1+6HfoqKGERvgkQ6Ti0Q0nLYVaGadjKHpGC+OHHLEdfXu0oAp
aMTkX4oohUBy344/lI+IcaRgj+FrEgRcWZ4w219NyGG+qmRAtrm1c6K1zio+YEBI
Gwbne5/anhrHSbIA43a/Qn70z6z71WVaKUQ3OQ+AdCmVYMhaKys9SGwbKcTIhxvK
B0E3EC/hqs8KellpexbdYAh9dNNMoRYGT5/yAck4M/deXvcXAWf/PTg2Xe0BZBCj
kdVB24fxyvOH9jcfIsYdAgMBAAGjaDBmMB8GA1UdIwQYMBaAFN2PxV+PjPIVwqvo
yBs2awZ4vrc1MAkGA1UdEwQCMAAwGQYDVR0RBBIwEIIOc2VydmVyLmV4YW1wbGUw
HQYDVR0OBBYEFFykOo3VpptRF51kb/ngXzA+58/0MA0GCSqGSIb3DQEBCwUAA4IB
AQADyXk5kdvN6fIfjzIlFGgH1QJrKRToHPwL2jpUQkJD56JWERDYjC8RInLFoG1L
h+gkF1kYg4QMoo7yy78dqiHPjiwp54CJkGhoqyjrCAld7VzTtgsTH9aGOsYhqMjO
xPfjZNCMr5UE/DoYU5+5+FDOtgwiLvlSm0o29hAOIIdU/R3SVFOYHIkZWAElg2xe
SR4eCmA11NnlqyWv8heS5StgGVqQKSV2HSZhIbFb1itSxXjhqZgGdI5ZK7miMs7g
JLbXLvIHrDhfksiVFnj4ECbGZQWGQHJJBocmtgIjaqOoMNeL26symLLU5BX6o1H8
5TVIzWgjubE/GP7SAnw3ZKBV
-----END CERTIFICATE-----
"""
    crt = x509.load_pem_x509_certificate(pem_data)
    return crt

def get_key(uid: UUID) -> Key:
    resp = req.get(
        url=f"{KEYSTORE}/v1/keys/{uid}",
        params={
            'type': 'priv'
        }
    )
    json = resp.json()
    key = Key(
        uid,
        json["name"],
        Key_type(str(json["type"]).upper()),
        int(json["size"]),
        json["data"]
        )
    return key

@app.post("v1/sign/")
def sign_file(
    file: Annotated[UploadFile, File()],
    crt_id: Annotated[UUID, Form()],
    key_id: Annotated[UUID, Form()],
):
    filename= file.filename
    if filename:
        tokens = filename.split('.')
        if len(tokens) <= 2:
            raise HTTPException(status_code=403, detail=f"File extension is not one of the supported (pdf, xml)")
        suffix = tokens[-1]
        if suffix.lower() == "pdf":
            # TODO: get CRT
            # TODO: get KEY
            key = get_key(key_id)
            signer = signers.SimpleSigner.load
            timestamper = timestamps.HTTPTimeStamper(
                url='https://freetsa.org/tsr'
            )
            signature_meta = signers.PdfSignatureMetadata(
                field_name='Signature', md_algorithm='sha256',
                # Mark the signature as a PAdES signature
                subfilter=SigSeedSubFilter.PADES,
                # We'll also need a validation context
                # to fetch & embed revocation info.
                validation_context=ValidationContext(allow_fetching=True),
                # Embed relevant OCSP responses / CRLs (PAdES-LT)
                embed_validation_info=True,
                # Tell pyHanko to put in an extra DocumentTimeStamp
                # to kick off the PAdES-LTA timestamp chain.
                use_pades_lta=True
            )
            with open('input.pdf', 'rb') as inf:
                w = IncrementalPdfFileWriter(inf)
                with open('output.pdf', 'wb') as outf:
                    signers.sign_pdf(
                        w, signature_meta=signature_meta, signer=signer,
                        timestamper=timestamper, output=outf
                    )

    else:
        raise HTTPException(status_code=403, detail=f"Invalid type of key for verifying data. AES key can't be used to validate certificate")

    #Go to key_store
    # get the response:
    # if no key -> return 403
    # else work 
    return {
        "file_size": file.size,
        "file_content_type": file.content_type,
        "message": f"file {file.filename} is valid"
    }


@app.post("v1/verify/")
def verify_file(
    file: Annotated[UploadFile, File()],
    key_id: Annotated[UUID, Form()],
):
    pprint(key_id)
    key = get_key(key_id)
    match key._type:
        case Key_type.aes:
            raise HTTPException(status_code=403, detail=f"Invalid type of key for verifying data. AES key can't be used to validate certificate")
        case Key_type.rsa, Key_type.ecdsa:
            load_private_key_from_pemder_data(key.data)
            vc = ValidationContext()
            # if PDF
            with open(file, 'rb') as doc:
                r = PdfFileReader(doc)
                
                sig = r.embedded_signatures[0]
                status = validate_pdf_signature(sig, vc)
                print(status.pretty_print_details())
            
            
        case _:
            raise HTTPException(status_code=501, detail=f"Not implemented handling of {key._type}")
    #Go to key_store
    # get the response:
    # if no key -> return 403
    # else work 
    return {
        "file_size": file.size,
        "file_content_type": file.content_type,
        "message": f"file {file.filename} is valid"
    }

from fastapi import FastAPI, requests
from fastapi.exceptions import HTTPException

from data_classes import Key_request, Key_type

import requests

from Crypto.Random import get_random_bytes
from Crypto.PublicKey import RSA
from Crypto.PublicKey import ECC

from generator_config import settings

app = FastAPI()

KEYS_SIZES_MAX = {
    "AES": settings.AES_MAX,
    "RSA": settings.RSA_MAX,
    "ECDSA": settings.ECDSA_MAX,
}

KEYS_SIZES_MIN = {
    "AES": settings.AES_MIN,
    "RSA": settings.RSA_MIN,
    "ECDSA": settings.ECDSA_MIN,
}

KEYSTORE_URL = settings.KEYSTORE_URL


@app.post("/v1/generate_key")
async def get_aes_cipher(key_request: Key_request):
    if key_request.key_type == Key_type.AES:

        response_code = send_rsa_key(key_request)
        return handle_response_code(response_code, key_request)

    elif key_request.key_type == Key_type.RSA:

        response_code = send_rsa_key(key_request)
        return handle_response_code(response_code, key_request)

    elif key_request.key_type == Key_type.ECDSA:

        response_code = send_ecdsa_key(key_request)
        return handle_response_code(response_code, key_request)

    else:
        raise HTTPException(400, "Wrong key type, allowed: AES, RSA, ECDSA")


def handle_response_code(response_code: int, key_request: Key_request):
    if response_code == 200:
        return key_request
    if response_code == 400:
        raise HTTPException(
            400, "Key already exists. New key not added to keystore")
    raise HTTPException(500, "Internal error. Key not added to keystore")


def send_aes_key(key_request: Key_request):
    check_size_guard(key_request.key_size, Key_type.AES)
    key_bytes = get_random_bytes(key_request.key_size)

    files = {'data': ("key", key_bytes),
             "name": (None, key_request.key_name),
             "type": (None, key_request.key_type),
             "size": (None, key_request.key_size), }

    response = requests.post(KEYSTORE_URL, files=files)
    return response.status_code


def send_rsa_key(key_request: Key_request):
    check_size_guard(key_request.key_size, Key_type.AES)

    key = RSA.generate(key_request.key_size)
    private_key = key.export_key()

    print(private_key)

    files = {'data': ("key", private_key),
             "name": (None, key_request.key_name),
             "type": (None, key_request.key_type),
             "size": (None, key_request.key_size), }

    response = requests.post(KEYSTORE_URL, files=files)
    return response.status_code


def send_ecdsa_key(key_request: Key_request):
    size = key_request.key_size
    try:
        key = ECC.generate(curve=f"P-{size}")
    except KeyError:
        raise HTTPException(400, f"ECDSA key size not allowed. Allowed values: 192, 224, 256, 384, 521. Got: {size}")

    private_key = key.export_key(format="PEM")

    files = {'data': ("key", private_key),
             "name": (None, key_request.key_name),
             "type": (None, key_request.key_type),
             "size": (None, key_request.key_size), }
    response = requests.post(KEYSTORE_URL, files=files)
    return response.status_code


def check_size_guard(size: int, key_type: Key_type):
    if KEYS_SIZES_MAX[key_type] <= size:
        raise HTTPException(400, "Key size not allowed. Key size too big")
    if KEYS_SIZES_MIN[key_type] >= size:
        raise HTTPException(400, "Key size not allowed. Key size too small")

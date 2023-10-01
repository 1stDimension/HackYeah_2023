import pytest
from main import Key_request, app, send_aes_key, send_rsa_key, send_ecdsa_key

from fastapi.testclient import TestClient


client = TestClient(app)

AES_PAYLOAD = {
    "key_type": "AES",
    "key_size": 2048,
    "key_name": "TestAES"
}

RSA_PAYLOAD = {
    "key_type": "RSA",
    "key_size": 2048,
    "key_name": "TestRSA"
}

ECDSA_PAYLOAD = {
    "key_type": "ECDSA",
    "key_size": 521,
    "key_name": "TestECDSA"
}

KEY_REQUEST_AES = Key_request(key_type="AES", key_name="TestAES2", key_size=2048)
KEY_REQUEST_RSA = Key_request(key_type="RSA", key_name="TestRSA2", key_size=2048)
KEY_REQUEST_ECDSA = Key_request(key_type="ECDSA", key_name="TestECDSA5", key_size=522)

def test_aes_request():
    response = client.post("/v1/generate_key", json=AES_PAYLOAD)
    print(response.request.read())
    assert response.status_code == 200
    assert response.json() == AES_PAYLOAD


def test_rsa_key():
    response = client.post("/v1/generate_key", json=RSA_PAYLOAD)
    print(response.request.read())
    assert response.status_code == 200

## Integration tests

def test_add_aes_key():
    response = send_aes_key(KEY_REQUEST_AES)
    print(response)


def test_add_rsa_key():
    response = send_rsa_key(KEY_REQUEST_RSA)
    print(response)


def test_add_ecdsa_key():
    response = send_ecdsa_key(KEY_REQUEST_ECDSA)
    print(response)

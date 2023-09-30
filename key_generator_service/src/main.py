from fastapi import FastAPI, requests
from fastapi.exceptions import HTTPException
from pydantic import BaseModel
from enum import Enum
import requests

from Crypto.Random import get_random_bytes


app = FastAPI()


class Key_type(str, Enum):
    AES = "AES",
    RSA = "RSA",
    ECDSA = "ECDSA",


class Key_request(BaseModel):
    key_type: str
    key_name: str
    key_size: int


class Key(BaseModel):
    key_data: str
    key_type: str
    key_name: str
    key_size: int


KEYSTORE_HEADERS = {}

KEYSTORE_URL = "http://localhost:5000/v1/keys"

KEYS_SIZES_MAX = {
        "AES": 2048,
        }
KEYS_SIZES_MIN = {
        "AES": 20000}

@app.post("/v1/generate_key")
async def get_aes_cipher(key_request: Key_request):
    if key_request.key_type == Key_type.AES:
        return send_aes_code(key_request)
    else:
        raise HTTPException(400, "Wrong key type, allowed: AES, RSA, ECDSA")

def send_aes_code(key_request: Key_request):
    check_size_guard(key_request.key_size, Key_type.AES)
    key_bytes = get_random_bytes(key_request.key_size)

    files = {'data': ("key", key_bytes),
                                 "name": (None, key_request.key_name),
                                 "type": (None, key_request.key_type),
                                 "size": (None, key_request.key_size),}

    response = requests.post(KEYSTORE_URL, files=files)
    if response.status_code == 201:
        return key_request
    else:
        raise HTTPException(500, "Internal error. Key not addded to keystore")

def check_size_guard(size: int, key_type: Key_type):
    if KEYS_SIZES_MAX[key_type] <= size:
        raise HTTPException(400, "Key size not allowed. Key size too small")
    if KEYS_SIZES_MIN[key_type] <= size:
        raise HTTPException(400, "Key size not allowed. Key size too big")

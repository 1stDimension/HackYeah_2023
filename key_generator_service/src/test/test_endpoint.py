import pytest
from main import app

from fastapi.testclient import TestClient


client = TestClient(app)

AES_PAYLOAD = {
        "key_type": "AES",
        "key_size": 16,
        "key_name": "TestName"
        }

def test_read_item():
    response = client.post("/v1/generate_key", json=AES_PAYLOAD)
    assert response.status_code == 200
    assert response.json() == AES_PAYLOAD

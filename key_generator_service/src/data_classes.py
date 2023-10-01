from enum import Enum
from pydantic import BaseModel


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

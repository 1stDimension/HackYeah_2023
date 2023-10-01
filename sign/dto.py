from dataclasses import dataclass
from typing import AnyStr
from enum import Enum
import uuid

class Key_type(str, Enum):
    aes = "AES"
    rsa = "RSA"
    ecdsa = "ECDSA"

@dataclass
class KeyMetadata():
    _id: uuid.UUID
    name: str
    _type: Key_type
    size: int

@dataclass
class Key(KeyMetadata):
    data: AnyStr
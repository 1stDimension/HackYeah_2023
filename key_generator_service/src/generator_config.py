from pydantic_settings import BaseSettings

class Settings(BaseSettings):
    AES_MAX: int = 4092
    AES_MIN: int = 128

    RSA_MAX: int = 4092
    RSA_MIN: int = 2048

    ECDSA_MAX: int = 192
    ECDSA_MIN: int = 521

    KEYSTORE_URL: str = "http://localhost:5000/v1/keys"

settings = Settings()

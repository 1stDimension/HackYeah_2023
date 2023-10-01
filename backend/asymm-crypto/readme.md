# HackYeah Backend - Asymmetric Encryptor-Decryptor
## Howto
```
docker buildx build --tag=hackyeah-asymmenc:latest --load .
docker run -dit -e HACKYEAH__KEYSTORE=http://localhost:5000/ -p 5001:80 hackyeah-asymmenc:latest
```

Make sure to replace http://localhost:5000/ with the actual keystore URL.

This makes the encryptor-decryptor available at http://localhost:5001/

If you want to increase logging verbosity, run this instead:

```
docker run -dit -e HACKYEAH__KEYSTORE=http://localhost:5000/ -e LOGGING__LOGLEVEL__DEFAULT=Trace -p 5001:80 hackyeah-asymmenc:latest
```

## Endpoints
### `POST /v1/encrypt`
**Content type**: multipart/form-data
**field** | **type** | **comment**
----------|----------|------------
`key`     | `str`    | ID of the key held in keystore, used to encrypt the data.
`file`    | file     | Contents of the file to encrypt.

#### Returns
Encrypted file
### `POST /v1/decrypt`
**Content type**: multipart/form-data
**field** | **type** | **comment**
----------|----------|------------
`key`     | `str`    | ID of the key held in keystore, used to decrypt the data.
`file`    | file     | Contents of the file to decrypt.

#### Returns
Decrypted file

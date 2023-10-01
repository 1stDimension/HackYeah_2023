# HackYeah Backend - Keystore
## Howto
```
docker buildx build --tag=hackyeah-keystore:latest --load .
docker volume create hackyeah-keystore
docker run -dit -v hackyeah-keystore:/data -p 5000:80 hackyeah-keystore:latest
```

This makes the keystore available at http://localhost:5000/

If you want to increase logging verbosity, run this instead:

```
docker run -dit -e LOGGING__LOGLEVEL__DEFAULT=Trace -e HACKYEAH__LOGSENSITIVE=true -v hackyeah-keystore:/data -p 5000:80 hackyeah-keystore:latest
```

## Endpoints
### `POST /v1/keys`
**Content type**: multipart/form-data
**field** | **type** | **comment**
----------|----------|------------
`name`    | `str`    | User-facing name of the key
`type`    | `str`    | `AES`/`RSA`/`ECDSA`
`size`    | `uint`   | Bit-size of the key, must be >0
`data`    | file     | Data containing the key, either PEM for asymmetric keys (such as RSA/ECDSA), or binary contents for symmetric keys (such as AES)

#### Returns
JSON:  
**field** | **type** | **description**
----------|----------|----------------
`id`      | `str`    | ID of the created key
`name`    | `str`    | User-facing name of the key

### `GET /v1/keys`
Returns an array containing all defined keys

#### Returns
JSON array of:  
**field** | **type** | **description**
----------|----------|----------------
`id`      | `str`    | ID of the created key
`name`    | `str`    | User-facing name of the key

### `GET /v1/keys/:id`
Returns symmetric key data for a given ID

#### Returns
JSON:  
**field** | **type** | **description**
----------|----------|----------------
`id`      | `str`    | ID of the key
`name`    | `str`    | Name of the key
`type`    | `str`    | Type of the key (`AES`/`RSA`/`ECDSA`)
`size`    | `uint`   | Bit-size of the key
`data`    | `str`    | PEM data for asymmetric keys, base64-encoded binary data for symmetric keys

### `GET /v1/keys/:id/public`
Returns public key data from an asymmetric key for a given ID

#### Returns
JSON:  
**field** | **type** | **description**
----------|----------|----------------
`id`      | `str`    | ID of the key
`name`    | `str`    | Name of the key
`type`    | `str`    | Type of the key (`AES`/`RSA`/`ECDSA`)
`size`    | `uint`   | Bit-size of the key
`data`    | `str`    | PEM data for asymmetric keys, base64-encoded binary data for symmetric keys

### `GET /v1/keys/:id/private`
Returns private key data from an asymmetric key for a given ID

#### Returns
JSON:  
**field** | **type** | **description**
----------|----------|----------------
`id`      | `str`    | ID of the key
`name`    | `str`    | Name of the key
`type`    | `str`    | Type of the key (`AES`/`RSA`/`ECDSA`)
`size`    | `uint`   | Bit-size of the key
`data`    | `str`    | PEM data for asymmetric keys, base64-encoded binary data for symmetric keys

### `POST /v1/certficates`
**Content type**: multipart/form-data
**field** | **type** | **comment**
----------|----------|------------
`data`    | file     | Data containing the certificate encoded in PEM; Can have private key appended as PEM data

#### Returns
JSON:  
**field**    | **type** | **description**
-------------|----------|----------------
`id`         | `str`    | ID of the created key
`cn`         | `str`    | Common name of the certificate
`thumbprint` | `str`    | Thumbprint of the certificate
`has_key`    | `bool`   | Whether the certificate has a private key associated with it

### `GET /v1/certificates`
Returns an array containing all defined certificates

#### Returns
JSON array of:  
**field**    | **type** | **description**
-------------|----------|----------------
`id`         | `str`    | ID of the created key
`cn`         | `str`    | Common name of the certificate
`thumbprint` | `str`    | Thumbprint of the certificate
`has_key`    | `bool`   | Whether the certificate has a private key associated with it

### `GET /v1/certificates/:id`
Returns a PEM-encoded certificate, with private key if applicable

#### Returns
PEM bundle

### `GET /v1/certificates/by-thumbprint/:thumbprint`
Returns a PEM-encoded certificate, with private key if applicable

#### Returns
PEM bundle

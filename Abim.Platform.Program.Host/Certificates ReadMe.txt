netsh http delete sslcert ipport=0.0.0.0:5020
netsh http add sslcert ipport=0.0.0.0:5020 appid={12345678-db90-4b66-8b01-88f7af2e36bf} certhash=c006a88b7f373924564a844d6d299d38ae203f17

WINDOWS  10
netsh http add urlacl url="https://*:5040/" user=everyone
netsh http add sslcert hostnameport=KEVIN-DESKTOP:5040 certhash=c006a88b7f373924564a844d6d299d38ae203f17 appid={12345678-db90-4b66-8b01-88f7af2e36bf} certstore=my

Creating the CA
All the certificates are issue by Certifying Authority or a CA. So for creating an CA, fire up the “Visual Studio Command Line Tools” and run as administrator. I would recommend create a new folder and browse to that so that things dont mix up.
Type in the following command to generate a new CA certificate.

makecert -n "CN=VineetYadav.com" -r -pe -a sha512 -cy authority -sv VineetYadavPKey.pvk VineetYadav.comCA.cer
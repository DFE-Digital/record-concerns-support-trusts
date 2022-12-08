# Setup Firefox
echo "{
    \"policies\": {
        \"Certificates\": {
            \"Install\": [
            	\"aspnetcore-localhost-https.crt\"
            ]
        }
    }
}" > policies.json

dotnet dev-certs https -ep localhost.crt --format PEM

sudo mv policies.json /usr/lib/firefox/distribution/
mkdir -p ~/.mozilla/certificates
cp localhost.crt ~/.mozilla/certificates/aspnetcore-localhost-https.crt

# Trust Edge/Chrome
certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n localhost -i ./localhost.crt
certutil -d sql:$HOME/.pki/nssdb -A -t "C,," -n localhost -i ./localhost.crt

# Trust dotnet-to-dotnet (.pem extension is important here)
sudo cp localhost.crt /usr/lib/ssl/certs/aspnetcore-https-localhost.pem

# Cleanup
rm localhost.crt
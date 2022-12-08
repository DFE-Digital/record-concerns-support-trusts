ls
echo "Copying to policies.json"
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

echo "Creating the cert"
dotnet dev-certs https -ep localhost.crt --format PEM
echo "Dev cert created"

echo "Setting up firefox"
sudo mv policies.json /usr/lib/firefox/distribution/
mkdir -p ~/.mozilla/certificates
cp localhost.crt ~/.mozilla/certificates/aspnetcore-localhost-https.crt

echo "Setting up chrome"
# Trust Edge/Chrome
certutil -d sql:$HOME/.pki/nssdb -N
#certutil -d sql:$HOME/.pki/nssdb -A -t "P,," -n localhost -i ./localhost.crt
#certutil -d sql:$HOME/.pki/nssdb -A -t "C,," -n localhost -i ./localhost.crt

echo "setting up .net trust"
# Trust dotnet-to-dotnet (.pem extension is important here)
sudo cp localhost.crt /usr/lib/ssl/certs/aspnetcore-https-localhost.pem

# Cleanup
echo "Cleanup"
rm localhost.crt
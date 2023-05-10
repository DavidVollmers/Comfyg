FROM dvol/comfyg:latest

RUN apt update && apt install nodejs npm -y

RUN npm i -g azurite

COPY azurite.Dockerfile.sh .

CMD ./azurite.Dockerfile.sh

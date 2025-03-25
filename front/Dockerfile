# Etapa 1: Construção da aplicação
FROM node:20 AS build

# Define o diretório de trabalho
WORKDIR /src

# Copia o package.json e package-lock.json para instalar as dependências
COPY package*.json .

# Instala as dependências
RUN npm install

# Copia o restante do código fonte
COPY . .

# Construa a aplicação para produção
RUN npm run build

# Etapa 2: Servir a aplicação
FROM nginx:alpine

# Copia os arquivos de build da etapa anterior para o diretório de servidor do nginx
COPY --from=build /src/build /usr/share/nginx/html

# Expõe a porta 80 para acesso à aplicação
EXPOSE 80

# Inicia o nginx para servir a aplicação
CMD ["nginx", "-g", "daemon off;"]

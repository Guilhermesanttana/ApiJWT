# 🔐 ApiJWT

Uma API de exemplo desenvolvida em **.NET Core** com autenticação via **JWT (JSON Web Token)** e suporte a **refresh token**, utilizando **SQL Server** como banco de dados.

---

## 📌 Funcionalidades

- Autenticação via JWT com tempo de expiração configurável (padrão: 2 horas)
- Suporte a refresh token (dependente de banco de dados)
- Controle de acesso baseado em roles
- Exemplo de rota protegida por autenticação JWT

---

## 🔧 Tecnologias Utilizadas

- [.NET Core](https://dotnet.microsoft.com/)
- SQL Server
- JWT (via `System.IdentityModel.Tokens.Jwt`)
- ASP.NET Core Web API

---

## 🔐 Autenticação

### 📥 Rota de login

`POST api/AuthController/Auth`

### 🔑 Payload de exemplo:

```json
{
  "User": "Guilherme",
  "Roles": ["ROLE000001"],
  "ApiKey": "AF37D202-33FF-4BD1-B26F-6C8F9F15A31F"
}

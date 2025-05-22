# AutoInsight API

## 🚀 Sobre o Projeto

A **AutoInsight API** é uma API RESTful desenvolvida em ASP.NET Core .NET 9.0 para gestão inteligente de pátios e frotas de motocicletas. A API fornece endpoints completos para gerenciamento de pátios, funcionários, veículos e suas relações, com integração ao banco de dados Oracle e documentação automática via OpenAPI/Scalar.

## 👥 Equipe de Desenvolvimento

| Nome | RM | E-mail | GitHub | LinkedIn |
|------|-------|---------|---------|----------|
| Arthur Vieira Mariano | RM554742 | arthvm@proton.me | [@arthvm](https://github.com/arthvm) | [arthvm](https://linkedin.com/in/arthvm/) |
| Guilherme Henrique Maggiorini | RM554745 | guimaggiorini@gmail.com | [@guimaggiorini](https://github.com/guimaggiorini) | [guimaggiorini](https://linkedin.com/in/guimaggiorini/) |
| Ian Rossato Braga | RM554989 | ian007953@gmail.com | [@iannrb](https://github.com/iannrb) | [ianrossato](https://linkedin.com/in/ianrossato/) |

## 🛠️ Tecnologias Utilizadas

### Stack Principal
- **.NET 9.0** - Framework principal
- **ASP.NET Core** - Minimal API
- **Entity Framework Core 9.0.4** - ORM
- **Oracle Database** - Banco de dados principal
- **AutoMapper 14.0.0** - Mapeamento de objetos
- **Scalar 2.3.0** - Documentação da API

### Arquitetura
- **Minimal API** - Implementação de rotas
- **Repository Pattern** - Abstração de acesso a dados
- **DTOs** - Transferência de dados
- **Migrations** - Controle de versão do banco

## 🗄️ Estrutura do Banco de Dados

O projeto utiliza Entity Framework Core com Oracle Database e inclui as seguintes entidades:

- **Yards** - Pátios de motocicletas
- **Vehicles** - Veículos/motocicletas
- **YardEmployees** - Funcionários dos pátios
- **YardVehicles** - Relação entre pátios e veículos
- **Addresses** - Endereços
- **QRCodes** - Códigos QR para identificação
- **Bookings** - Reservas
- **Models** - Modelos de motocicletas
- **EmployeeInvites** - Convites para funcionários

## 🚀 Como Executar o Projeto

### Pré-requisitos

- .NET 9.0 SDK
- Oracle Database
- Git

### Instalação

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/autoinsight-labs/aspnet.git
   cd aspnet
   ```

2. **Configure a variável de ambiente:**
   ```bash
   # Crie um arquivo .env na raiz do projeto
   echo "ORACLE_CONNECTION_STRING=sua_connection_string_aqui" > .env
   ```

3. **Restaure as dependências:**
   ```bash
   dotnet restore
   ```

4. **Execute as migrations:**
   ```bash
   dotnet ef database update
   ```

5. **Execute o projeto:**
   ```bash
   dotnet run
   ```

6. **Acesse a documentação:**
   - Swagger/Scalar: `http://localhost:5100/scalar/v1`
   - OpenAPI: `http://localhost:5100/openapi/v1.json`

## 📋 Rotas da API

### Health Check
| Método | Endpoint | Descrição | Retorno |
|--------|----------|-----------|---------|
| GET | `/health` | Verificação de saúde da API | 200 OK |

### 🏢 Yards (Pátios)
| Método | Endpoint | Descrição | Parâmetros | Retorno |
|--------|----------|-----------|------------|---------|
| GET | `/yard` | Lista pátios com paginação | `pageNumber`, `pageSize` | 200 OK, 400 BadRequest |
| GET | `/yard/{id}` | Busca pátio por ID | `id` (path) | 200 OK, 404 NotFound |
| POST | `/yard` | Cria novo pátio | Body: `YardDTO` | 201 Created, 500 InternalServerError |
| PUT | `/yard/{id}` | Atualiza pátio existente | `id` (path), Body: `YardDTO` | 200 OK, 404 NotFound |
| DELETE | `/yard/{id}` | Remove pátio | `id` (path) | 204 NoContent, 404 NotFound |

### 👥 Employees (Funcionários)
| Método | Endpoint | Descrição | Parâmetros | Retorno |
|--------|----------|-----------|------------|---------|
| GET | `/yard/{id}/employees` | Lista funcionários do pátio | `id` (path), `pageNumber`, `pageSize` | 200 OK, 400 BadRequest, 404 NotFound |
| GET | `/yard/{id}/employees/{employeeId}` | Busca funcionário específico | `id`, `employeeId` (path) | 200 OK, 404 NotFound |
| POST | `/yard/{id}/employees` | Adiciona funcionário ao pátio | `id` (path), Body: `YardEmployeeDTO` | 201 Created, 404 NotFound |
| PUT | `/yard/{id}/employees/{employeeId}` | Atualiza funcionário | `id`, `employeeId` (path), Body: `YardEmployeeDTO` | 200 OK, 400 BadRequest, 404 NotFound |
| DELETE | `/yard/{id}/employees/{employeeId}` | Remove funcionário | `id`, `employeeId` (path) | 204 NoContent, 404 NotFound |

### 🏍️ Vehicles (Veículos)
| Método | Endpoint | Descrição | Parâmetros | Retorno |
|--------|----------|-----------|------------|---------|
| GET | `/vehicle` | Busca veículo por QR Code | `qrCodeId` (query) | 200 OK, 404 NotFound |
| GET | `/vehicle/{id}` | Busca veículo por ID | `id` (path) | 200 OK, 404 NotFound |

### 🏍️ Yard Vehicles (Veículos do Pátio)
| Método | Endpoint | Descrição | Parâmetros | Retorno |
|--------|----------|-----------|------------|---------|
| GET | `/yard/{id}/vehicles` | Lista veículos do pátio | `id` (path), `pageNumber`, `pageSize` | 200 OK, 400 BadRequest, 404 NotFound |
| GET | `/yard/{id}/vehicles/{yardVehicleId}` | Busca veículo específico do pátio | `id`, `yardVehicleId` (path) | 200 OK, 400 BadRequest, 404 NotFound |
| POST | `/yard/{id}/vehicles` | Adiciona veículo ao pátio | `id` (path), Body: `YardVehicleDTO` | 201 Created, 400 BadRequest, 404 NotFound |
| PUT | `/yard/{id}/vehicles/{yardVehicleId}` | Atualiza veículo do pátio | `id`, `yardVehicleId` (path), Body: `YardVehicleDTO` | 200 OK, 400 BadRequest, 404 NotFound |

## 🎯 Funcionalidades Implementadas

### ✅ CRUD Completo
- **Yards**: Criação, leitura, atualização e exclusão de pátios
- **Employees**: Gestão completa de funcionários por pátio
- **Vehicles**: Consulta e gestão de veículos

### ✅ Rotas Parametrizadas
- **Query Parameters**: Paginação (`pageNumber`, `pageSize`), filtros (`qrCodeId`)
- **Path Parameters**: IDs de recursos (`id`, `employeeId`, `yardVehicleId`)

### ✅ Retornos HTTP Adequados
- **200 OK**: Sucesso em consultas e atualizações
- **201 Created**: Criação de novos recursos
- **204 NoContent**: Exclusão bem-sucedida
- **400 BadRequest**: Parâmetros inválidos
- **404 NotFound**: Recurso não encontrado
- **500 InternalServerError**: Erro interno do servidor

### ✅ Integração com Oracle
- Entity Framework Core com provider Oracle
- Migrations para criação e versionamento das tabelas
- Connection string via variável de ambiente

### ✅ Documentação OpenAPI
- Scalar UI para interface gráfica moderna
- Documentação automática de todas as rotas
- Schemas de request/response definidos

## 📊 Exemplo de Uso

```bash
# Listar pátios
curl -X GET "http://localhost:5100/yard?pageNumber=1&pageSize=10"

# Criar novo pátio
curl http://localhost:5100/yard \
  --request POST \
  --header 'Content-Type: application/json' \
  --data '{
  "address": {
    "country": "BR",
    "state": "SP",
    "city": "São Paulo",
    "zipCode": "01311-000",
    "neighborhood": "Bela Vista",
    "complement": "Avenida Paulista, 1106"
  },
  "ownerId": "d51bqcy66s8o1lokzvquy6ux"
}'

# Buscar veículo por QR Code
curl -X GET "http://localhost:5100/vehicle?qrCodeId=QR123"
```

## 📝 Padrões de Desenvolvimento

- **Repository Pattern** para abstração de dados
- **DTOs** para transferência segura de dados
- **AutoMapper** para mapeamento automático
- **Minimal APIs** para performance otimizada
- **Environment Variables** para configurações sensíveis

## 📄 Licença

Este projeto foi desenvolvido para fins acadêmicos como parte do challenge da Mottu FIAP.
# AutoInsight API

## 🚀 Sobre o Projeto

A **AutoInsight API** é uma API RESTful desenvolvida em ASP.NET Core (.NET 9.0) para a gestão inteligente de pátios e frotas de motocicletas. A API fornece endpoints completos para o gerenciamento de pátios, funcionários, veículos e seus relacionamentos, com integração ao banco de dados Oracle e documentação automática via OpenAPI/Scalar.

## 👥 Equipe de Desenvolvimento

| Nome                      | RM       | E-mail                  | GitHub                                      | LinkedIn                                            |
| ------------------------- | -------- | ----------------------- | ------------------------------------------- | --------------------------------------------------- |
| Arthur Vieira Mariano     | RM554742 | arthvm@proton.me        | [@arthvm](https://github.com/arthvm)        | [arthvm](https://linkedin.com/in/arthvm/)           |
| Guilherme Henrique Maggiorini | RM554745 | guimaggiorini@gmail.com | [@guimaggiorini](https://github.com/guimaggiorini) | [guimaggiorini](https://linkedin.com/in/guimaggiorini/) |
| Ian Rossato Braga         | RM554989 | ian007953@gmail.com     | [@iannrb](https://github.com/iannrb)        | [ianrossato](https://linkedin.com/in/ianrossato/)   |

## 🏛️ Justificativa da Arquitetura

A arquitetura desta API foi projetada para ser moderna, performática e de fácil manutenção, utilizando padrões e tecnologias consolidadas no ecossistema .NET.

-   **Minimal APIs**: Escolhida para reduzir o *boilerplate* e aumentar a performance. Com a abordagem de Minimal APIs, o código fica mais limpo, direto e otimizado para cenários de alta performance, como o de uma API REST.

-   **Repository Pattern**: Utilizado para abstrair a camada de acesso a dados. Isso desacopla a lógica de negócio das tecnologias de persistência (neste caso, o Entity Framework Core), facilitando a manutenção, a testabilidade e a possibilidade de trocar o provedor de banco de dados no futuro sem grande impacto.

-   **DTOs (Data Transfer Objects)**: Adotados para criar um contrato claro e seguro entre o cliente e a API. O uso de DTOs impede a exposição de detalhes internos dos modelos de domínio (*over-posting*), melhora a segurança e permite que a API evolua sem quebrar os clientes.

-   **Injeção de Dependência**: O projeto faz uso extensivo da injeção de dependência nativa do ASP.NET Core para gerenciar o ciclo de vida dos serviços, como os repositórios e mappers, promovendo um código mais modular e testável.

## 🛠️ Tecnologias Utilizadas

### Stack Principal
- **.NET 9.0**: Framework principal
- **ASP.NET Core**: Minimal API
- **Entity Framework Core 9.0.4**: ORM
- **Oracle Database**: Banco de dados principal
- **AutoMapper 14.0.0**: Mapeamento de objetos
- **Scalar 2.3.0**: Documentação da API
- **FluentValidation 11.3.1**: Validação de requisições
- **DotNetEnv 3.1.1**: Carregamento de variáveis de ambiente

### Arquitetura
- **Minimal API**: Implementação de rotas
- **Repository Pattern**: Abstração de acesso a dados
- **DTOs**: Transferência de dados
- **Migrations**: Controle de versão do banco de dados

## 🗄️ Estrutura do Banco de Dados

O projeto utiliza Entity Framework Core com Oracle Database e inclui as seguintes entidades:

- **Yards**: Pátios de motocicletas
- **Vehicles**: Veículos/motocicletas
- **YardEmployees**: Funcionários do pátio
- **YardVehicles**: Relação entre pátios e veículos
- **Addresses**: Endereços
- **QRCodes**: Códigos QR para identificação
- **Bookings**: Reservas
- **Models**: Modelos de motocicletas
- **EmployeeInvites**: Convites para funcionários

## 🚀 Como Executar o Projeto

### Pré-requisitos

- .NET 9.0 SDK
- .NET Entity Framework CLI
- Oracle Database
- Git
- Docker (opcional, para execução em contêiner)

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
   - Scalar UI: `http://localhost:5100/scalar`
   - OpenAPI JSON: `http://localhost:5100/openapi/v1.json`

### Executando com Docker

1. **Construa a imagem Docker:**
   ```bash
   docker build -t autoinsight-api .
   ```

2. **Execute o contêiner:**
   ```bash
   docker run -p 8080:8080 -e ORACLE_CONNECTION_STRING="sua_connection_string_aqui" autoinsight-api
   ```

## 📋 Rotas da API

### Health Check
| Método | Endpoint  | Descrição                 | Retorno |
| ------ | --------- | ------------------------- | ------- |
| GET    | `/health` | Verificação de saúde da API | 200 OK  |

### 🏢 Pátios (Yards)
| Método | Endpoint      | Descrição                  | Parâmetros                      | Retorno                        |
| ------ | ------------- | -------------------------- | ------------------------------- | ------------------------------ |
| GET    | `/yards`      | Lista pátios com paginação | `pageNumber`, `pageSize`        | 200 OK, 400 BadRequest         |
| GET    | `/yards/{id}` | Busca pátio por ID         | `id` (path)                     | 200 OK, 404 NotFound           |
| POST   | `/yards`      | Cria novo pátio            | Body: `CreateYardDto`           | 201 Created, 500 InternalServerError |
| PATCH  | `/yards/{id}` | Atualiza pátio existente   | `id` (path), Body: `YardDto`    | 200 OK, 404 NotFound           |
| DELETE | `/yards/{id}` | Remove pátio               | `id` (path)                     | 204 NoContent, 404 NotFound    |

### 👥 Funcionários do Pátio (Yard Employees)
| Método | Endpoint                         | Descrição                  | Parâmetros                                     | Retorno                                |
| ------ | -------------------------------- | -------------------------- | ---------------------------------------------- | -------------------------------------- |
| GET    | `/yards/{id}/employees`          | Lista funcionários do pátio | `id` (path), `pageNumber`, `pageSize`          | 200 OK, 400 BadRequest, 404 NotFound   |
| GET    | `/yards/{id}/employees/{employeeId}` | Busca funcionário específico | `id`, `employeeId` (path)                      | 200 OK, 404 NotFound                   |
| PATCH  | `/yards/{id}/employees/{employeeId}` | Atualiza funcionário       | `id`, `employeeId` (path), Body: `YardEmployeeDto` | 200 OK, 400 BadRequest, 404 NotFound   |
| DELETE | `/yards/{id}/employees/{employeeId}` | Remove funcionário         | `id`, `employeeId` (path)                      | 204 NoContent, 404 NotFound            |

### 🏍️ Veículos (Vehicles)
| Método | Endpoint     | Descrição                 | Parâmetros           | Retorno              |
| ------ | ------------ | ------------------------- | -------------------- | -------------------- |
| GET    | `/vehicles`  | Busca veículo por QR Code | `qrCodeId` (query)   | 200 OK, 404 NotFound |
| GET    | `/vehicles/{id}` | Busca veículo por ID      | `id` (path)          | 200 OK, 404 NotFound |

### 🏍️ Veículos do Pátio (Yard Vehicles)
| Método | Endpoint                           | Descrição                    | Parâmetros                                      | Retorno                                |
| ------ | ---------------------------------- | ---------------------------- | ----------------------------------------------- | -------------------------------------- |
| GET    | `/yards/{id}/vehicles`             | Lista veículos do pátio      | `id` (path), `pageNumber`, `pageSize`           | 200 OK, 400 BadRequest, 404 NotFound   |
| GET    | `/yards/{id}/vehicles/{yardVehicleId}` | Busca veículo específico do pátio | `id`, `yardVehicleId` (path)                    | 200 OK, 400 BadRequest, 404 NotFound   |
| POST   | `/yards/{id}/vehicles`             | Adiciona veículo ao pátio    | `id` (path), Body: `CreateYardVehicleDto`       | 201 Created, 400 BadRequest, 404 NotFound |
| PATCH  | `/yards/{id}/vehicles/{yardVehicleId}` | Atualiza veículo no pátio    | `id`, `yardVehicleId` (path), Body: `YardVehicleDto` | 200 OK, 400 BadRequest, 404 NotFound   |

### ✉️ Convites (Invites)
| Método | Endpoint                   | Descrição                        | Parâmetros                           | Retorno                                |
| ------ | -------------------------- | -------------------------------- | ------------------------------------ | -------------------------------------- |
| POST   | `/yards/{id}/invites`      | Cria convite de funcionário      | `id` (path), Body: `CreateYardEmployeeDto` | 201 Created, 404 NotFound, 409 Conflict |
| GET    | `/yards/{id}/invites`      | Lista convites do pátio          | `id` (path), `pageNumber`, `pageSize`    | 200 OK, 400 BadRequest, 404 NotFound   |
| POST   | `/invites/{token}/accept`  | Aceita convite                   | `token` (path), Body: `AcceptInviteDto`  | 200 OK, 404 NotFound, 409 Conflict      |
| POST   | `/invites/{token}/reject`  | Rejeita convite                  | `token` (path)                       | 204 NoContent, 404 NotFound, 409 Conflict |
| GET    | `/invites/user/{userId}`   | Lista convites aceitos do usuário | `userId` (path), `pageNumber`, `pageSize`  | 200 OK, 400 BadRequest                 |
| GET    | `/invites/email/{email}`   | Lista convites pendentes por e-mail | `email` (path), `pageNumber`, `pageSize` | 200 OK, 400 BadRequest                 |

## 📊 Exemplos de Uso

A seguir, alguns exemplos de como interagir com a API utilizando `curl`.

### Criar um novo pátio

```bash
curl -X POST http://localhost:5100/yards \\
  -H "Content-Type: application/json" \\
  -d '{
    "ownerId": "usr_exemplo_001",
    "address": {
        "country": "BR",
        "state": "SP",
        "city": "São Paulo",
        "zipCode": "01311-000",
        "neighborhood": "Bela Vista",
        "complement": "Av. Paulista, 1106"
    }
}'
```

### Adicionar um veículo a um pátio (criando um novo veículo)

```bash
curl -X POST http://localhost:5100/yards/{id_do_patio}/vehicles \\
  -H "Content-Type: application/json" \\
  -d '{
    "status": "WAITING",
    "enteredAt": "2025-09-26T10:00:00Z",
    "vehicle": {
        "plate": "BRA2E19",
        "model": {
            "name": "Honda Biz 125",
            "year": 2023
        },
        "userId": "usr_exemplo_002"
    }
}'
```

### Listar os funcionários de um pátio

```bash
curl -X GET "http://localhost:5100/yards/{id_do_patio}/employees?pageNumber=1&pageSize=5"
```

## ✅ Testes Manuais

Como o projeto ainda não possui uma suíte de testes automatizados, os testes podem ser realizados manualmente para garantir a qualidade e o funcionamento esperado da API.

### Utilizando a Documentação Interativa (Scalar)

A forma mais simples de testar os endpoints é através da interface do Scalar, que é gerada automaticamente a partir da especificação OpenAPI.

1.  **Execute a API** localmente (`dotnet run`).
2.  **Acesse a URL** da documentação no seu navegador: `http://localhost:5100/scalar`.
3.  **Navegue pelos endpoints** na interface, preencha os parâmetros e corpos de requisição necessários e clique em "Execute" para enviar a requisição e observar a resposta.

### Utilizando Ferramentas de Cliente HTTP (Postman, Insomnia, curl)

Você também pode utilizar qualquer cliente HTTP de sua preferência para testar a API.

-   **Importe a especificação OpenAPI**: A maioria das ferramentas modernas, como Postman e Insomnia, permite importar a especificação OpenAPI para criar uma coleção de requisições automaticamente. A especificação está disponível em `http://localhost:5100/openapi/v1.json`.
-   **Crie as requisições manualmente**: Utilize os exemplos de `curl` fornecidos na seção anterior como base para montar suas requisições.

## 📝 Padrões de Desenvolvimento

- **Repository Pattern** para abstração de dados
- **DTOs** para transferência segura de dados
- **AutoMapper** para mapeamento automático
- **Minimal APIs** para performance otimizada
- **Variáveis de Ambiente** para configurações sensíveis

## 📄 Licença

Este projeto foi desenvolvido para fins acadêmicos como parte do challenge da Mottu FIAP.

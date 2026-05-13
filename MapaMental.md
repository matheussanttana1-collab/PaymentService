# 🗺️ Roadmap de Implementação: PaymentService

Siga a ordem de construção de dentro para fora (Domain -> Application -> Infra -> API).

## 1. 🟣 PaymentService.Domain (O Coração)
Sem dependências externas. Contém apenas regras de negócio puras.

- [ ] `PaymentService.Domain/`
  - [x] `Entities/`
    - [x] `Order.cs` *(Controle do pedido: Id, Amount, Status, TenantId)*
    - [x] `WebhookEvent.cs` *(Inbox pattern para salvar eventos da Stripe/Mercado Pago)*
  - [x] `Interfaces/`
    - [x] `IOrderRepository.cs` *(Contrato do banco de dados)*
    - [x] `IWebhookEventRepository.cs` *(Contrato do banco de dados)*
    - [x] `IPaymentGateway.cs` *(Contrato para gerar cobranças externas)*

---

## 2. 🔵 PaymentService.Application (O Cérebro)
Depende de: `PaymentService.Domain`. Orquestra as regras de negócio.

- [ ] `PaymentService.Application/`
  - [ ] `DTOs/`
    - [ ] `PixPaymentRequest.cs` *(Dados que a BarberCode envia)*
    - [ ] `PixPaymentResponse.cs` *(Retorno com o QR Code)*
  - [ ] `Events/`
    - [ ] `PagamentoAprovadoEvent.cs` *(O evento que será jogado no RabbitMQ)*
  - [ ] `UseCases/`
    - [ ] `ICriarPagamentoPixUseCase.cs`
    - [ ] `CriarPagamentoPixUseCase.cs` *(Fluxo Síncrono: Chama Gateway -> Salva Pendente -> Retorna App)*
    - [ ] `IProcessarWebhookUseCase.cs`
    - [ ] `ProcessarWebhookUseCase.cs` *(Fluxo Assíncrono: Lê Webhook -> Atualiza Banco -> Publica RabbitMQ)*

---

## 3. 🟢 PaymentService.Infrastructure (Os Braços)
Depende de: `PaymentService.Application`. Fala com BD, APIS externas e RabbitMQ.
*Pacotes NuGet: `Microsoft.EntityFrameworkCore`, `MassTransit.RabbitMQ`*

- [ ] `PaymentService.Infrastructure/`
  - [ ] `Persistence/`
    - [ ] `PaymentDbContext.cs` *(Configuração do Entity Framework)*
    - [ ] `Repositories/`
      - [ ] `OrderRepository.cs` *(Implementa IOrderRepository)*
      - [ ] `WebhookEventRepository.cs` *(Implementa IWebhookEventRepository)*
  - [ ] `Gateways/`
    - [ ] `MercadoPagoGateway.cs` *(Implementa IPaymentGateway fazendo o POST HTTP real)*
  - [ ] `Configuration/`
    - [ ] `DependencyInjection.cs` *(Classe estática que registra todos os serviços acima e o MassTransit)*

---

## 4. 🔴 PaymentService.API (A Porta de Entrada)
Depende de: `PaymentService.Application` e `PaymentService.Infrastructure`.

- [ ] `PaymentService.API/`
  - [ ] `Controllers/`
    - [ ] `PaymentsController.cs` *(Endpoint: `POST /api/v1/payments/pix`)*
    - [ ] `WebhooksController.cs` *(Endpoint: `POST /api/v1/webhooks/mercadopago`)*
  - [ ] `appsettings.json` *(Strings de conexão do MySQL, URLs e credenciais de teste)*
  - [ ] `Program.cs` *(Onde chamamos o `AddInfrastructure()` e subimos a aplicação)*
# AbySalto - High-level dizajn sustava (online retail platforma)

## 1. Sažetak
- Cilj: globalna online maloprodajna platforma s milijunima korisnika dnevno i više prodajnih kanala (web, mobile, marketplace, B2B)
- Pristup: modularni servisni sustav (domain-based), stateless servisi, event-driven integracije, jasne granice odgovornosti
- Fokus implementacije u ovom zadatku: Cart Web API servis + PostgreSQL + odabrani zahtjevi (auth, health, docker, CI)

## 2. Ključni zahtjevi
- Skalabilnost i visoki promet
  - horizontalno skaliranje servisa
  - cache, rate limiting, backpressure
- Sigurne transakcije i zaštita podataka
  - OIDC/JWT, least privilege, enkripcija, audit
- Real-time obrada
  - event streaming i asinkrona obrada
- Dva cross-funkcionalna tima
  - jasna podjela domena, API contracti, observability standardi

## 3. Arhitektura
### 3.1 Komponente
- Klijenti
  - Web shop (SPA/SSR)
  - Mobilne aplikacije
  - Marketplace konektori
  - B2B partneri
- Edge sloj
  - CDN + WAF
  - API Gateway (routing, auth enforcement, rate limiting, request limits)
- Aplikacijski sloj
  - Identity/Auth servis (OIDC)
  - Catalog servis
  - Pricing/Promotions servis
  - Inventory servis
  - Cart servis
  - Order servis
  - Payment orchestrator (PSP integracije)
  - Shipping/Fulfillment servis
  - Integration servis (Porezna uprava, ERP, CRM, marketplace)
  - Notification servis
- Data sloj
  - PostgreSQL (transactional domene)
  - Redis (cache, session-like data, rate-limit state)
  - Event streaming (Kafka/RabbitMQ)
  - Search (OpenSearch/Elasticsearch)
- Observability
  - Metrike (Prometheus) + dashboard (Grafana)
  - Logovi (Loki/ELK)
  - Tracing (OpenTelemetry + Jaeger/Tempo)

### 3.2 Komunikacija između komponenti
- Sinkrono
  - Klijent -> Gateway -> servis (REST ili gRPC)
  - primarno za kratke transakcije i čitanja
- Asinkrono
  - eventi preko message brokera (npr. OrderCreated, StockReserved)
  - consumeri rade projekcije, notifikacije, integracije
- Outbox pattern
  - servis piše event u outbox tablicu u istoj DB transakciji
  - background publisher sigurno šalje event u broker

## 4. Odabir tehnologija (prijedlog)
- Backend: .NET 8 Web API
- DB: PostgreSQL
- Cache: Redis
- Messaging: Kafka (visok throughput, event log) ili RabbitMQ (task queue)
- Gateway: Kong/NGINX ili cloud-native gateway
- Deploy: Kubernetes + Helm
- CI/CD: GitHub Actions ili GitLab CI
- Observability: OpenTelemetry + Prometheus/Grafana + centralni logovi

## 5. Strategija skaliranja
- Stateless servisi
  - servis ne drži state u memoriji (osim kratkog per-request)
  - state je u DB/Redis, pa je horizontalno skaliranje trivijalno
- Cache strategija
  - Redis za hot read i za ubrzanje (npr. cart read-through cache)
  - cache invalidation preko događaja (CartUpdated)
- Database scaling
  - database-per-service (smanjuje contention)
  - read replica gdje ima smisla
  - partitioning po regiji ili tenant-u ako promet i veličina rastu
- Rate limiting i zaštita
  - limit po IP i po useru na gatewayu
  - circuit breaker i timeout prema vanjskim ovisnostima
- Real-time
  - event stream + consumeri

## 6. Sigurnost i autentifikacija
- Autentifikacija
  - OAuth2/OIDC provider (Keycloak/Auth0/Entra ID) u produkciji
  - JWT access token prema API servisu
- Autorizacija
  - scope/claim-based (npr. cart:read, cart:write)
- Zaštita podataka
  - TLS svugdje
  - enkripcija podataka u mirovanju (DB encryption, KMS)
  - secrets u secret manageru, ne u repou
- Payment sigurnost
  - kartični podaci ne prolaze kroz naš backend (PSP hosted page ili tokenization)
- Audit
  - bilježiti promjene stanja i kritične akcije (tko, kada, što)

## 7. Ključne komponente i odgovornosti (primjeri)
- Cart servis
  - CRUD košarice i stavki
  - validacije i izračuni subtotal-a
  - optimistic concurrency ili idempotency za sigurnost promjena
- Inventory servis
  - dostupnost i rezervacije
- Pricing servis
  - cijene, popusti, kuponi
- Order servis
  - state machine narudžbe
- Integration servis
  - izolira vanjske integracije, retry, dead-letter queue
- Observability sloj
  - health, metrics, logs, traces, alerting

## 8. Integracije s vanjskim servisima (npr. Porezna uprava)
- Principi
  - izolirati integracije u dedicated Integration servis
  - retry + exponential backoff
  - idempotency (isti zahtjev ne smije proizvesti duple efekte)
  - DLQ za ručnu obradu kada se retry iscrpi
- Primjer toka
  - OrderPaid event -> Integration servis -> fiskalizacija -> emit FiskalizacijaCompleted event

## 9. Monitoring i alerting
- Healthcheck
  - liveness: proces radi
  - readiness: DB i ovisnosti dostupne
- Metrike
  - RPS, latencija p95/p99, error rate
  - DB pool, cache hit rate, queue lag
- Logovi
  - structured logging + correlation id
- Tracing
  - OpenTelemetry u gatewayu i servisima
- Alerting
  - SLO bazirani alertovi (availability i latency)
  - paging samo za kritične tokove (checkout, payment)

## 10. Plan isporuke koda
- CI
  - build + test na svaki PR
  - static analiza i format
- CD
  - docker image tag po commit-u
  - deploy dev/stage automatski, prod uz approval
  - DB migracije kontrolirano (job)
- Branching
  - trunk-based
  - kratke feature grane + PR review
  - main uvijek releasable

## 11. Minimalna implementacija u ovom repou (Cart API)
- .NET 8 Web API servis
- PostgreSQL baza
- EF Core migracije
- Auth
  - dev token endpoint (za demo)
  - struktura spremna za OIDC/JWT u produkciji
- Health endpoints
  - /health/live
  - /health/ready
- Docker compose za lokalno pokretanje

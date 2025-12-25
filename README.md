# AbySalto - Cart API (minimalna implementacija)

## Preduvjeti
- Docker + Docker Compose

## Pokretanje
- U rootu repozitorija:
  - docker compose -f docker/docker-compose.yml up --build

## Endpointi
- Swagger:
  - http://localhost:8080/swagger

## Health
- Liveness:
  - GET http://localhost:8080/health/live
- Readiness:
  - GET http://localhost:8080/health/ready

## Napomene
- Auth je JWT (dev issuer). U pravom sustavu bi se koristio OIDC provider.
- Baza je PostgreSQL. Migracije se pokrecu pri startu u dev modu.

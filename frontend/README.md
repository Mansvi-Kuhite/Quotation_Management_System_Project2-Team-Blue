# QMS Frontend (React SPA)

## Run

```bash
cd frontend
npm install
npm run dev
```

## Backend Connection

- Default API base in dev: `http://localhost:5001/api` (direct call, no proxy dependency).
- Override API base URL when backend runs on a different port:

```bash
VITE_API_BASE_URL=http://localhost:5001/api npm run dev
```

## Demo Users

- `salesrep / 123`
- `salesmanager / 123`
- `admin / 123`

## Docker (From Project Root)

Run the complete stack (React + API + SQL Server + Redis):

```bash
docker-compose up --build
```

Frontend: `http://localhost:5173`  
API Swagger: `http://localhost:5000/swagger`

# Recipe World

A full-stack .NET 8 + React recipe app with world cuisines, user auth, saved recipes, and ingredient-based suggestions (powered by [TheMealDB](https://www.themealdb.com/)).

## Features

- **Auth**: Register and login (email + password, JWT).
- **Recipes**: Search by name, browse by cuisine, view recipe details.
- **Save recipes**: Logged-in users can save/unsave recipes and view them in My Recipes.
- **Suggestions**: Enter ingredients you have; get recipe ideas with match/missing ingredients.

## Run locally

### Backend

```bash
cd Backend
dotnet run
```

API: http://localhost:5064 (or https://localhost:7200 if using HTTPS profile).  
On first run, the DB is created and seeded with:

- **demo@recipes.com** / **Demo123!**
- **chef@worldcuisine.com** / **Chef123!**

(Each has a few pre-saved recipes.)

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Open http://localhost:5173. The dev server proxies `/api` to the backend; ensure the backend URL in `frontend/vite.config.ts` matches your backend (default `http://localhost:5064`).

### Tests

```bash
cd Backend.Tests
dotnet test
```

## Project layout

- **Backend** – ASP.NET Core Web API, EF Core (SQLite), JWT auth, TheMealDB client, suggestion service.
- **frontend** – React (Vite) + TypeScript, React Router, auth context, pages for Home, Cuisines, Recipe detail, My Recipes, Suggestions.

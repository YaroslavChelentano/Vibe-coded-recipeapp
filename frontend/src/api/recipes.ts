import { apiFetch } from './client';

export interface RecipeDto {
  id?: number;
  externalId?: string | null;
  title: string;
  area?: string | null;
  category?: string | null;
  thumbnailUrl?: string | null;
  instructions?: string | null;
  source: string;
  ingredientsJson?: string | null;
  isSaved?: boolean;
}

export function searchRecipes(query: string) {
  return apiFetch<RecipeDto[]>(`/recipes/search?query=${encodeURIComponent(query)}`);
}

export function getCuisines() {
  return apiFetch<string[]>('/recipes/cuisines');
}

export function getRecipesByCuisine(area: string, count = 20) {
  return apiFetch<RecipeDto[]>(`/recipes/by-cuisine?area=${encodeURIComponent(area)}&count=${count}`);
}

export function getRecipeById(id: string) {
  return apiFetch<RecipeDto>(`/recipes/${encodeURIComponent(id)}`);
}

export function getMyRecipes() {
  return apiFetch<RecipeDto[]>('/user/recipes');
}

export interface SaveRecipeRequest {
  id?: number;
  externalId?: string;
  title: string;
  area?: string;
  category?: string;
  thumbnailUrl?: string;
  instructions?: string;
  ingredientsJson?: string;
}

export function saveRecipe(data: SaveRecipeRequest) {
  return apiFetch<RecipeDto>('/user/recipes/save', {
    method: 'POST',
    body: JSON.stringify(data),
  });
}

export function unsaveRecipe(recipeId: number) {
  return apiFetch<void>(`/user/recipes/save/${recipeId}`, { method: 'DELETE' });
}

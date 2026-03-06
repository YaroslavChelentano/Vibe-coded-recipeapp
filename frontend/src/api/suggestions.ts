import { apiFetch } from './client';
import type { RecipeDto } from './recipes';

export interface SuggestionResultDto {
  recipe: RecipeDto;
  score: number;
  matchingIngredients: string[];
  missingIngredients: string[];
}

export function getSuggestionsFromIngredients(ingredients: string[], maxResults = 15) {
  return apiFetch<SuggestionResultDto[]>('/suggestions/from-ingredients', {
    method: 'POST',
    body: JSON.stringify({ ingredients, maxResults }),
  });
}

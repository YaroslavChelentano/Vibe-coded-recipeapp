import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getSuggestionsFromIngredients } from '../api/suggestions';
import type { SuggestionResultDto } from '../api/suggestions';
import { RecipeCard } from '../components/RecipeCard';

export function SuggestionsPage() {
  const [inputText, setInputText] = useState('');
  const [results, setResults] = useState<SuggestionResultDto[]>([]);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const ingredients = inputText.split(/[,;]/).map((s) => s.trim()).filter(Boolean);
    if (ingredients.length === 0) return;
    setLoading(true);
    try {
      const list = await getSuggestionsFromIngredients(ingredients);
      setResults(list);
    } catch {
      setResults([]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page">
      <h1>Recipes from your ingredients</h1>
      <p>Enter ingredients you have (comma or semicolon separated), and we'll suggest recipes.</p>
      <form onSubmit={handleSubmit} className="suggestions-form">
        <textarea
          placeholder="e.g. chicken, rice, garlic, onion"
          value={inputText}
          onChange={(e) => setInputText(e.target.value)}
          rows={3}
        />
        <button type="submit" disabled={loading}>{loading ? 'Finding recipes...' : 'Get suggestions'}</button>
      </form>
      {results.length > 0 && (
        <section className="suggestions-results">
          <h2>Suggestions</h2>
          {results.map((s, i) => (
            <div key={s.recipe.externalId ?? s.recipe.id ?? i} className="suggestion-item">
              <RecipeCard recipe={s.recipe} onOpen={() => navigate(`/recipes/${s.recipe.externalId ?? s.recipe.id}`)} />
              <div className="suggestion-meta">
                <span>Match score: {s.score}</span>
                {s.matchingIngredients.length > 0 && <span>You have: {s.matchingIngredients.join(', ')}</span>}
                {s.missingIngredients.length > 0 && <span>Missing: {s.missingIngredients.slice(0, 8).join(', ')}{s.missingIngredients.length > 8 ? '...' : ''}</span>}
              </div>
            </div>
          ))}
        </section>
      )}
    </div>
  );
}

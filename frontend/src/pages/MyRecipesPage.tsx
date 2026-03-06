import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { getMyRecipes, unsaveRecipe } from '../api/recipes';
import type { RecipeDto } from '../api/recipes';
import { RecipeCard } from '../components/RecipeCard';

export function MyRecipesPage() {
  const [recipes, setRecipes] = useState<RecipeDto[]>([]);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  const load = () => {
    getMyRecipes()
      .then(setRecipes)
      .catch(() => setRecipes([]))
      .finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  const handleUnsave = async (r: RecipeDto) => {
    if (r.id == null) return;
    try {
      await unsaveRecipe(r.id);
      setRecipes((prev) => prev.filter((x) => x.id !== r.id));
    } catch {
      // ignore
    }
  };

  if (loading) return <div className="page">Loading...</div>;

  return (
    <div className="page">
      <h1>My Saved Recipes</h1>
      {recipes.length === 0 ? (
        <p>No saved recipes yet. Save some from the recipe detail page!</p>
      ) : (
        <div className="recipe-grid">
          {recipes.map((r) => (
            <div key={r.id ?? r.externalId ?? r.title} className="recipe-card-wrapper">
              <RecipeCard recipe={r} onOpen={() => navigate(`/recipes/${r.externalId ?? r.id}`)} />
              <button type="button" className="unsave-btn" onClick={() => handleUnsave(r)}>Remove</button>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

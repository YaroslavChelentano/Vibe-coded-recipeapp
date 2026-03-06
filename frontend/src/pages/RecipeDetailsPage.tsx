import { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { getRecipeById } from '../api/recipes';
import { saveRecipe, unsaveRecipe } from '../api/recipes';
import type { RecipeDto } from '../api/recipes';
import { useAuth } from '../context/AuthContext';

export function RecipeDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { token } = useAuth();
  const [recipe, setRecipe] = useState<RecipeDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [saveError, setSaveError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    getRecipeById(id)
      .then(setRecipe)
      .catch(() => setRecipe(null))
      .finally(() => setLoading(false));
  }, [id]);

  const handleSave = async () => {
    if (!recipe || !token) return;
    setSaving(true);
    setSaveError(null);
    try {
      const updated = await saveRecipe({
        externalId: recipe.externalId ?? undefined,
        id: recipe.id && recipe.id > 0 ? recipe.id : undefined,
        title: recipe.title,
        area: recipe.area ?? undefined,
        category: recipe.category ?? undefined,
        thumbnailUrl: recipe.thumbnailUrl ?? undefined,
        instructions: recipe.instructions ?? undefined,
        ingredientsJson: recipe.ingredientsJson ?? undefined,
      });
      setRecipe({ ...recipe, ...updated, isSaved: true });
    } catch (err) {
      setSaveError(err instanceof Error ? err.message : 'Failed to save recipe');
    } finally {
      setSaving(false);
    }
  };

  const handleUnsave = async () => {
    if (!recipe?.id || !token) return;
    setSaving(true);
    try {
      await unsaveRecipe(recipe.id);
      setRecipe({ ...recipe, isSaved: false });
    } catch {
      // ignore
    } finally {
      setSaving(false);
    }
  };

  if (loading) return <div className="page">Loading...</div>;
  if (!recipe) return <div className="page">Recipe not found.</div>;

  const ingredients: string[] = recipe.ingredientsJson
    ? (() => { try { return JSON.parse(recipe.ingredientsJson!); } catch { return []; } })()
    : [];

  return (
    <div className="page recipe-detail">
      <button type="button" className="back" onClick={() => navigate(-1)}>← Back</button>
      <div className="recipe-detail-header">
        <h1>{recipe.title}</h1>
        {recipe.area && <span className="badge">{recipe.area}</span>}
        {token && (
          recipe.isSaved ? (
            <button type="button" onClick={handleUnsave} disabled={saving}>Unsave</button>
          ) : (
            <>
              <button type="button" onClick={handleSave} disabled={saving}>{saving ? 'Saving...' : 'Save recipe'}</button>
              {saveError && <span className="save-error">{saveError}</span>}
            </>
          )
        )}
      </div>
      {recipe.thumbnailUrl && <img src={recipe.thumbnailUrl} alt={recipe.title} className="recipe-detail-img" />}
      {ingredients.length > 0 && (
        <section>
          <h2>Ingredients</h2>
          <ul className="ingredients-list">
            {ingredients.map((ing, i) => <li key={i}>{ing}</li>)}
          </ul>
        </section>
      )}
      {recipe.instructions && (
        <section>
          <h2>Instructions</h2>
          <p className="instructions">{recipe.instructions}</p>
        </section>
      )}
    </div>
  );
}

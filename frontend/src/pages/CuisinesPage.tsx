import { useEffect, useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { getCuisines, getRecipesByCuisine } from '../api/recipes';
import type { RecipeDto } from '../api/recipes';
import { RecipeCard } from '../components/RecipeCard';

export function CuisinesPage() {
  const [cuisines, setCuisines] = useState<string[]>([]);
  const [recipes, setRecipes] = useState<RecipeDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();
  const area = searchParams.get('area') ?? '';

  useEffect(() => {
    getCuisines().then(setCuisines).catch(() => setCuisines([]));
  }, []);

  useEffect(() => {
    if (!area) {
      setRecipes([]);
      return;
    }
    setLoading(true);
    getRecipesByCuisine(area)
      .then(setRecipes)
      .catch(() => setRecipes([]))
      .finally(() => setLoading(false));
  }, [area]);

  return (
    <div className="page">
      <h1>Cuisines of the World</h1>
      <div className="cuisine-chips">
        {cuisines.map((c) => (
          <button
            key={c}
            type="button"
            className={area === c ? 'active' : ''}
            onClick={() => setSearchParams({ area: c })}
          >
            {c}
          </button>
        ))}
      </div>
      {area && (
        <section>
          <h2>{area}</h2>
          {loading ? <p>Loading...</p> : (
            <div className="recipe-grid">
              {recipes.map((r) => (
                <RecipeCard key={r.externalId ?? r.id ?? r.title} recipe={r} onOpen={() => navigate(`/recipes/${r.externalId ?? r.id}`)} />
              ))}
            </div>
          )}
        </section>
      )}
    </div>
  );
}

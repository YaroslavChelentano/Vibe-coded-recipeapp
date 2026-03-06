import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { searchRecipes, getRecipesByCuisine } from '../api/recipes';
import type { RecipeDto } from '../api/recipes';
import { RecipeCard } from '../components/RecipeCard';

const FEATURED_CUISINES = ['Italian', 'Mexican', 'Japanese'] as const;
const FEATURED_COUNT = 6;

export function HomePage() {
  const [query, setQuery] = useState('');
  const [results, setResults] = useState<RecipeDto[]>([]);
  const [featured, setFeatured] = useState<RecipeDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [featuredLoading, setFeaturedLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    let cancelled = false;
    setFeaturedLoading(true);
    Promise.all(
      FEATURED_CUISINES.map((area) => getRecipesByCuisine(area, 2))
    )
      .then((lists) => {
        if (cancelled) return;
        const combined = lists.flat();
        const seen = new Set<string>();
        const unique = combined.filter((r) => {
          const key = r.externalId ?? r.id?.toString() ?? r.title;
          if (seen.has(key)) return false;
          seen.add(key);
          return true;
        });
        setFeatured(unique.slice(0, FEATURED_COUNT));
      })
      .catch(() => setFeatured([]))
      .finally(() => { if (!cancelled) setFeaturedLoading(false); });
    return () => { cancelled = true; };
  }, []);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!query.trim()) return;
    setLoading(true);
    try {
      const list = await searchRecipes(query.trim());
      setResults(list);
    } catch (err) {
      setResults([]);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="page home-page">
      <section className="hero">
        <h1>Recipes from Around the World</h1>
        <p>Discover cuisines and get recipe ideas from the ingredients you have.</p>
        <form onSubmit={handleSearch} className="search-form">
          <input
            type="text"
            placeholder="Search recipes..."
            value={query}
            onChange={(e) => setQuery(e.target.value)}
          />
          <button type="submit" disabled={loading}>{loading ? 'Searching...' : 'Search'}</button>
        </form>
      </section>
      {results.length > 0 ? (
        <section className="home-section">
          <h2>Search results</h2>
          <div className="recipe-grid">
            {results.map((r) => (
              <RecipeCard key={r.externalId ?? r.id ?? r.title} recipe={r} onOpen={() => navigate(`/recipes/${r.externalId ?? r.id}`)} />
            ))}
          </div>
        </section>
      ) : (
        <section className="home-section featured-section">
          <h2>Explore by cuisine</h2>
          <p className="section-desc">Try these recipes from Italian, Mexican, and Japanese cuisines.</p>
          {featuredLoading ? (
            <p className="section-loading">Loading recipes…</p>
          ) : featured.length > 0 ? (
            <div className="recipe-grid">
              {featured.map((r) => (
                <RecipeCard key={r.externalId ?? r.id ?? r.title} recipe={r} onOpen={() => navigate(`/recipes/${r.externalId ?? r.id}`)} />
              ))}
            </div>
          ) : null}
        </section>
      )}
    </div>
  );
}

import type { RecipeDto } from '../api/recipes';

interface RecipeCardProps {
  recipe: RecipeDto;
  onOpen: () => void;
}

export function RecipeCard({ recipe, onOpen }: RecipeCardProps) {
  return (
    <article className="recipe-card" onClick={onOpen} onKeyDown={(e) => e.key === 'Enter' && onOpen()} role="button" tabIndex={0}>
      {recipe.thumbnailUrl && <img src={recipe.thumbnailUrl} alt={recipe.title} />}
      <div className="recipe-card-body">
        <h3>{recipe.title}</h3>
        {recipe.area && <span className="badge">{recipe.area}</span>}
      </div>
    </article>
  );
}

import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { Navbar } from './components/Navbar';
import { ProtectedRoute } from './components/ProtectedRoute';
import { HomePage } from './pages/HomePage';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { CuisinesPage } from './pages/CuisinesPage';
import { RecipeDetailsPage } from './pages/RecipeDetailsPage';
import { MyRecipesPage } from './pages/MyRecipesPage';
import { SuggestionsPage } from './pages/SuggestionsPage';
import './App.css';

function Layout({ children }: { children: React.ReactNode }) {
  return (
    <div className="layout">
      <Navbar />
      <main>{children}</main>
    </div>
  );
}

export default function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Layout>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/cuisines" element={<CuisinesPage />} />
            <Route path="/recipes/:id" element={<RecipeDetailsPage />} />
            <Route path="/my-recipes" element={<ProtectedRoute><MyRecipesPage /></ProtectedRoute>} />
            <Route path="/suggestions" element={<ProtectedRoute><SuggestionsPage /></ProtectedRoute>} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </Layout>
      </AuthProvider>
    </BrowserRouter>
  );
}

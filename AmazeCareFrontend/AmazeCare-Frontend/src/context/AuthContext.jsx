import { createContext, useContext, useReducer, useEffect } from 'react';

// ─── Initial state ────────────────────────────────────────────────────────────
const initialState = {
  token: null,
  role: null,
  roleSpecificId: null,
  userId: null,
  fullName: null,
  isAuthenticated: false,
};

// ─── Reducer ──────────────────────────────────────────────────────────────────
function authReducer(state, action) {
  switch (action.type) {
    case 'LOGIN':
      return {
        ...state,
        token: action.payload.token,
        role: action.payload.role,
        roleSpecificId: action.payload.roleSpecificId,
        userId: action.payload.userId,
        fullName: action.payload.fullName,
        isAuthenticated: true,
      };
    case 'LOGOUT':
      return { ...initialState };
    default:
      return state;
  }
}

// ─── Context ──────────────────────────────────────────────────────────────────
const AuthContext = createContext(null);

// ─── Provider ─────────────────────────────────────────────────────────────────
export function AuthProvider({ children }) {
  const [state, dispatch] = useReducer(authReducer, initialState, () => {
    // Rehydrate from localStorage on first load so a page refresh
    // doesn't log the user out.
    try {
      const token = localStorage.getItem('amazecare_token');
      const role = localStorage.getItem('amazecare_role');
      const roleSpecificId = localStorage.getItem('amazecare_role_specific_id');
      const userId = localStorage.getItem('amazecare_user_id');
      const fullName = localStorage.getItem('amazecare_full_name');

      if (token && role) {
        return {
          token,
          role,
          roleSpecificId: roleSpecificId ? Number(roleSpecificId) : null,
          userId: userId ? Number(userId) : null,
          fullName: fullName ?? null,
          isAuthenticated: true,
        };
      }
    } catch {
      // localStorage not available (SSR, private mode edge cases)
    }
    return initialState;
  });

  // Keep localStorage in sync whenever state changes
  useEffect(() => {
    if (state.isAuthenticated) {
      localStorage.setItem('amazecare_token', state.token);
      localStorage.setItem('amazecare_role', state.role);
      localStorage.setItem('amazecare_role_specific_id', state.roleSpecificId ?? '');
      localStorage.setItem('amazecare_user_id', state.userId ?? '');
      localStorage.setItem('amazecare_full_name', state.fullName ?? '');
    } else {
      localStorage.removeItem('amazecare_token');
      localStorage.removeItem('amazecare_role');
      localStorage.removeItem('amazecare_role_specific_id');
      localStorage.removeItem('amazecare_user_id');
      localStorage.removeItem('amazecare_full_name');
    }
  }, [state.isAuthenticated, state.token]);

  function login(payload) {
    dispatch({ type: 'LOGIN', payload });
  }

  function logout() {
    dispatch({ type: 'LOGOUT' });
  }

  return (
    <AuthContext.Provider value={{ ...state, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// ─── Hook ─────────────────────────────────────────────────────────────────────
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used inside <AuthProvider>');
  return ctx;
}
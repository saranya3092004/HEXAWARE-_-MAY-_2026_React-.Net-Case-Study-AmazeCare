import { useState, useCallback } from 'react';

export function useAsync(asyncFn) {
  const [state, setState] = useState({
    data: null,
    loading: false,
    error: null,
  });

  const execute = useCallback(
    async (...args) => {
      setState({ data: null, loading: true, error: null });
      try {
        const result = await asyncFn(...args);
        setState({ data: result.data, loading: false, error: null });
        return result.data;
      } catch (err) {
        setState({ data: null, loading: false, error: err.message });
        throw err;
      }
    },
    [asyncFn]
  );

  return { ...state, execute };
}
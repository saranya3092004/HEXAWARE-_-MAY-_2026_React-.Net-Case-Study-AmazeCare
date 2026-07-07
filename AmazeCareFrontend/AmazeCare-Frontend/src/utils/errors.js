// Errors that came through the axios interceptor are plain Error objects
// with messages we explicitly set — safe to show to users.
// Runtime errors (ReferenceError, TypeError, SyntaxError) are code bugs
// — never show these to users.
export function isUserFacingError(err) {
  return (
    err instanceof Error &&
    !(err instanceof ReferenceError) &&
    !(err instanceof TypeError) &&
    !(err instanceof SyntaxError) &&
    !(err instanceof RangeError)
  );
}

export const GENERIC_ERROR = 'Something went wrong. Please try again later.';
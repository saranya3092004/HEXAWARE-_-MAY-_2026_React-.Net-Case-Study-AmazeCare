
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
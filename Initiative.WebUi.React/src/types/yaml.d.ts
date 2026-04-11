declare module 'yaml' {
  export function parse<T = unknown>(source: string): T;
  export function stringify(value: unknown): string;
}

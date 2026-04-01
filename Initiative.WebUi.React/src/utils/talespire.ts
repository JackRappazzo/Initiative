// TaleSpire Symbiote API – minimal type declarations for the injected `TS` global.
// The full API is documented at https://symbiote-docs.talespire.com/api_doc_v0_1.md.html

interface TSRollDescriptor {
  name: string;
  roll: string;
}

interface TSStateDiceAPI {
  isValidRollString(rollStr: string): Promise<boolean>;
  makeRollDescriptors(rollString: string): Promise<TSRollDescriptor[]>;
  putDiceInTray(rollDescriptors: TSRollDescriptor[], quietResults: boolean): Promise<string>;
}

interface TSApi {
  dice: TSStateDiceAPI;
}

declare global {
  interface Window {
    TS?: TSApi;
  }
}

/** Returns true when the app is running inside a TaleSpire Symbiote WebView. */
export function isTaleSpire(): boolean {
  return typeof window !== 'undefined' && typeof window.TS !== 'undefined';
}

/**
 * Puts a dice roll into the TaleSpire dice tray.
 * @param expression A dice expression e.g. "2d6+3"
 * @param label      The name shown alongside the roll result in TaleSpire
 */
export async function rollInTray(expression: string, label: string): Promise<void> {
  if (!window.TS) return;
  const descriptors = await window.TS.dice.makeRollDescriptors(expression);
  // makeRollDescriptors splits on '/' — assign the label to each group
  const named = descriptors.map((d) => ({ ...d, name: label }));
  await window.TS.dice.putDiceInTray(named, false);
}

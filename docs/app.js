const languageButtons = document.querySelectorAll('[data-lang]');
const translatable = document.querySelectorAll('[data-es][data-en]');

function setLanguage(language) {
  const selected = language === 'en' ? 'en' : 'es';
  document.documentElement.lang = selected;
  translatable.forEach((element) => {
    element.innerHTML = element.dataset[selected];
  });
  languageButtons.forEach((button) => {
    button.setAttribute('aria-pressed', String(button.dataset.lang === selected));
  });
  document.title = selected === 'en'
    ? 'TerrainForge — Shape the world your way'
    : 'TerrainForge — Moldeá el mundo a tu manera';
  document.querySelector('meta[name="description"]').content = selected === 'en'
    ? 'TerrainForge: a free-form brush to sculpt, paint, and restore terrain in Valheim.'
    : 'TerrainForge: una brocha libre para esculpir, pintar y restaurar terrenos en Valheim.';
  try {
    localStorage.setItem('terrainforge-language', selected);
  } catch {
    // The language switch still works when browser storage is unavailable.
  }
}

languageButtons.forEach((button) => {
  button.addEventListener('click', () => setLanguage(button.dataset.lang));
});

let savedLanguage = null;
try {
  savedLanguage = localStorage.getItem('terrainforge-language');
} catch {
  // Spanish is the default when browser storage is unavailable.
}
const browserLanguage = navigator.language?.toLowerCase() || '';
const initialLanguage = savedLanguage || (browserLanguage.startsWith('en') ? 'en' : 'es');
setLanguage(initialLanguage);

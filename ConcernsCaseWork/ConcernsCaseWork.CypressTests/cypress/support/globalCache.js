export class GlobalCache {
    getItem(key) {
        return cy.task("getGlobalCacheItem", key);
    }

    setItem(name, value) {
        return cy.task("setGlobalCacheItem", { name, value });
    }
}
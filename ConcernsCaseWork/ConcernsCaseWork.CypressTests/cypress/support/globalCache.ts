export class GlobalCache {
    getItem(key: string) {
        return cy.task("getGlobalCacheItem", key);
    }

    setItem(name: string, value: string) {
        return cy.task("setGlobalCacheItem", { name, value });
    }
}
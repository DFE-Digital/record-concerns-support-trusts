const puppeteer = require('puppeteer')

// Use puppeteer to log in to azure AD
// Currently cypress has a problem loading the login page
module.exports.azureLogin = async function azureLogin(url, username, password) {
    const browser = await puppeteer.launch({
        headless: false,
    });

    const page = await browser.newPage();

    await page.goto(url);

    await page.waitForSelector(".moj-header__logo");

    const cookies = await page.cookies();

    const loginCookie = cookies.find(c => c.name === ".ConcernsCasework.Login");

    await browser.close();

    return loginCookie.value;
}
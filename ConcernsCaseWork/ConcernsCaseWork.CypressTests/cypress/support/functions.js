// ***********************************************
// This example functions.js shows you how to
// create various functions to be referenced within tests
// ***********************************************

let searchTerm =  ["10058372", "10060045", "10060278", 
                    "10058372", "10059732", "10060186", 
                    "10030314", "10080822", "10081341", 
                    "10058833", "1087656", "10058354", 
                    "10066108", "10058598", "10059919", 
                    "10057355", "10058295", "10059877"]; 

const trustRgx = new RegExp(/(School|Academy|Accrington)/, 'i');

//example: /case/5880/management"
Cypress.Commands.add("testVisitPage",(slug)=> {
	cy.visit(Cypress.env('url')+slug, { timeout: 30000 });
	cy.saveLocalStorage();
})

function testSelectRagRating(ragStatus) {
    switch (ragStatus) {
        case 'Red plus':
            cy.get('[id=rating-3]').click();
            break;
        case "Amber":
            cy.get('[id=rating]').click();
            break;
        case "Green":
            cy.get('[id=rating]').click();
            break;
        case "Red":
            cy.get('[id=rating-2]').click();
            break;
        default:
            cy.log("Could not make selection");
    }
}

//takes a css locator string eg: .govuk-radios .ragtag:nth-of-type(1) or [id=rating]
function randomSelectElement(el) {
    cy.get(el).its("length").then((rand) => {
        let num = Math.floor(Math.random() * rand); 
        cy.get(el).eq(num).click(); 
    })
}

//takes a css locator string eg: .govuk-radios .ragtag:nth-of-type(1) or [id=rating]
function randomSelectTrust(searchTerm) {
    //cy.get(searchTerm).its("length").then((rand) => {
        //let num = Math.floor(Math.random() * rand); 
        let num = Math.floor(Math.random()*searchTerm.length); 
        console.log(searchTerm[num]);
        //cy.get(searchTerm).eq(num).type()
        cy.get("#search").type(searchTerm[num])
        //return searchTerm[num];
        return searchTerm.eq([num]).text()
    
    //})
}

function SelectRandConcernTypeFn () {

/*
     let dict = new Object()

        //dict["concernType"] = ['[value="Compliance"]', '[value=Financial]','[value=260:Force majeure]' ,'[value=Governance]' ,'[value=Irregularity]' , ]
        dict["compliance"] = ['#sub-type-1', '#sub-type-2']
        dict["financial"] = ['#sub-type-3', '#sub-type-4', '#sub-type-5', '#sub-type-6']
        dict["force majeure"] = ['null']
        dict["governance"] = ["#sub-type-7", "#sub-type-8", "#sub-type-9", "#sub-type-10"]
        dict["irregularity"] = ["#sub-type-11", "#sub-type-12"]
*/

            //let num = Math.floor(Math.random()*concernType.length);
            //let num = Math.floor(Math.random()*dict.concernType.length); 
            let rand = 'null'


    let num = Math.floor(Math.random()*5); 
 //subNum = Math.floor(Math.random()*dict.concernType.length); 

    //let randKey = Object.keys(dict)[num]; //picks a random key from the dictonary
   // let result =  dict.randKey[Math.floor(Math.random()*randKey.length)];

             //console.log(result);
             //cy.get("#search").type(searchTerm[num] + "{enter}");


  switch (num) {
     case "1":
         cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
             cy.get($option).click()
         })
         cy.wrap('[for^="sub-type"]').its('length').as('len');
         rand = Math.floor(Math.random()*self.len);
         cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
         self.randResult.click();
         break;
     case "2":
         cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
             cy.get($option).click()
         })
         cy.wrap('[for^="sub-type"]').its('length').as('len');
         rand = Math.floor(Math.random()*self.len);
         cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
         self.randResult.click();
         break;
     case "3":
         cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
             cy.get($option).click()
         })
         cy.wrap('[for^="sub-type"]').its('length').as('len');
         rand = Math.floor(Math.random()*self.len);
         cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
         self.randResult.click();
         break;
     case "4":
         cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
             cy.get($option).click()
         })
         cy.wrap('[for^="sub-type"]').its('length').as('len');
         rand = Math.floor(Math.random()*self.len);
         cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
         self.randResult.click();
         break;
     case "5":
         cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
             cy.get($option).click()
         })
         cy.wrap('[for^="sub-type"]').its('length').as('len');
         rand = Math.floor(Math.random()*self.len);
         cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
         self.randResult.click();
         break;
     default:
         cy.log("Could not click option");
 }

 return 


}

function SelectRandConcernTypeDictFn () {


         let dict = new Object()
    
            dict["concernType"] = ['[value="Compliance"]', '[value=Financial]','[value=260:Force majeure]' ,'[value=Governance]' ,'[value=Irregularity]' , ]
            dict["compliance"] = ['#sub-type-1', '#sub-type-2']
            dict["financial"] = ['#sub-type-3', '#sub-type-4', '#sub-type-5', '#sub-type-6']
            dict["force majeure"] = ['null']
            dict["governance"] = ["#sub-type-7", "#sub-type-8", "#sub-type-9", "#sub-type-10"]
            dict["irregularity"] = ["#sub-type-11", "#sub-type-12"]

    
                //let num = Math.floor(Math.random()*concernType.length);
                let num = Math.floor(Math.random()*dict.concernType.length); 
                let rand = 'null'
    
    
     //let num = Math.floor(Math.random()*5); 
     //subNum = Math.floor(Math.random()*dict.concernType.length); 
    
        let randKey = Object.keys(dict)[num]; //picks a random key from the dictonary
        let result =  dict.randKey[Math.floor(Math.random()*randKey.length)];

        Object.keys(dict)[0].length
        dict.concernType[0]
    
                 //console.log(result);
                 //cy.get("#search").type(searchTerm[num] + "{enter}");
    
/*
      switch (num) {
         case "1":
             cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
                 cy.get($option).click()
             })
             cy.wrap('[for^="sub-type"]').its('length').as('len');
             rand = Math.floor(Math.random()*self.len);
             cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
             self.randResult.click();
             break;
         case "2":
             cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
                 cy.get($option).click()
             })
             cy.wrap('[for^="sub-type"]').its('length').as('len');
             rand = Math.floor(Math.random()*self.len);
             cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
             self.randResult.click();
             break;
         case "3":
             cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
                 cy.get($option).click()
             })
             cy.wrap('[for^="sub-type"]').its('length').as('len');
             rand = Math.floor(Math.random()*self.len);
             cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
             self.randResult.click();
             break;
         case "4":
             cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
                 cy.get($option).click()
             })
             cy.wrap('[for^="sub-type"]').its('length').as('len');
             rand = Math.floor(Math.random()*self.len);
             cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
             self.randResult.click();
             break;
         case "5":
             cy.get('.govuk-radios__item [value=Compliance]').then(($option) => {
                 cy.get($option).click()
             })
             cy.wrap('[for^="sub-type"]').its('length').as('len');
             rand = Math.floor(Math.random()*self.len);
             cy.wrap('[for^="sub-type"]').eq(rand-1).as('randResult');
             self.randResult.click();
             break;
         default:
             cy.log("Could not click option");
     }
*/    
     return 
    
    
    }

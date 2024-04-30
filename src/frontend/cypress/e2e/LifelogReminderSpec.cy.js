describe("Lifelog Reminder Test", () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638500415939286930","Iat":"638500403939286905","Nbf":null,"Scope":"","UserHash":"6tNxLo8XvHj4gwcvI1Yo31FjwuHKm1eB/GiBWncnwXs=","Claims":{"Role":"Normal"}},"Signature":"s9MhUfmMR3GY8SzkT4zI2476d6PO27XJxdqgT6w-moE"}'
    it('Send an email', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.wait(1000)
        cy.on('window:alert', (text) => {
            // Assert on the text of the alert
            expect(text).to.equal('Successfully Sent a Reminder to the User');
          });
    })
    it('Update the reminder form', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.wait(1000)
        cy.get('#user-setting').click()
        cy.wait(500)
        cy.get('submit').click();
        cy.on('window:alert', (text) => {
            // Assert on the text of the alert
            expect(text).to.equal('Successfully Sent a Reminder to the User');
        });
    })
})
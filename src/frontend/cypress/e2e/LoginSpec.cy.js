describe('Login Test', () => {
     let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638477345117710790","Iat":"638477333117710758","Nbf":null,"Scope":"","UserHash":"TestUser","Claims":{"Role":"Admin"}},"Signature":"lrs50BAplaJBayP3LEUCYscgQjjILbJrBVSOG4V9Jwc"}'


    it('Do a login', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()

        
        cy.wait(1000)
        //#endregion

        //#region Act
        

        cy.get('#username-input').type('TestUser');
        cy.get('#submit-credential-button').click();
        cy.wait(500)

        // enter OTP

        cy.get('#otp-input').type('rYdqph2v');
        cy.get('#submit-credential-button').click();
        
        cy.wait(500)
        
        //#endregion

        
        cy.window().then((win) => {
            const token = win.localStorage.getItem('token-local');
      
            // Assert that the token exists and is not null
            expect(token).to.exist;
            expect(token).to.not.be.null;
        })

        cy.window().its('localStorage').invoke('getItem', 'token-local').then((storedToken) => {
            // Parse the stored token as JSON
            const parsedToken = JSON.parse(storedToken);
      
            // Perform assertions on the token contents
            expect(parsedToken.Payload.Iss).to.equal('localhost');
            expect(parsedToken.Payload.Sub).to.equal('myApp');
            expect(parsedToken.Payload.Aud).to.equal('myApp');
            expect(parsedToken.Payload.Exp).to.equal('638477345117710790');
            expect(parsedToken.Payload.Iat).to.equal('638477333117710758');
            expect(parsedToken.Payload.UserHash).to.equal('TestUser');
            expect(parsedToken.Payload.Claims.Role).to.equal('Admin');
            
      
            // Clean up: Clear the token from localStorage
            window.localStorage.removeItem('token-local');
          });
        
        
    }) 
});
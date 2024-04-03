describe('Personal Note Test', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638477345117710790","Iat":"638477333117710758","Nbf":null,"Scope":"","UserHash":"TestUser","Claims":{"Role":"Admin"}},"Signature":"lrs50BAplaJBayP3LEUCYscgQjjILbJrBVSOG4V9Jwc"}'
    it('Create a new note', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.get('[data-testid = note-view]').click()
        cy.wait(1000)
        //#endregion

        //#region Act
        // Type into the note input field
        cy.get('#create-paragraph-input').click();
        cy.get('#create-paragraph-input').type('Hey');
        // Click the submit button
        cy.get('#submit-note-button').click();
        // Sleep for sometime
        cy.wait(1000)
        //#endregion

        //#region Assert
        // Check that the note content is displayed
        cy.get('#create-paragraph-input').should('contain', 'Hey');
        //#endregion

        // Clean up 
        cy.get('#note-delete-button').click()
        
    })

    it('Delete a new note', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.get('[data-testid = note-view]').click()
        cy.wait(1000)
        //#endregion

        //#region Act
        // Type into the note input field
        cy.get('#create-paragraph-input').click();
        cy.get('#create-paragraph-input').type('Test Deletion');
        // Click the submit button
        cy.get('#submit-note-button').click();
        // Sleep for sometime
        cy.wait(1000)
        // Select the delete button 
        cy.get('#note-delete-button').click()
        //#endregion

        //#region Assert
        // Check that the note content is displayed
        cy.get('#create-paragraph-input').should('contain', '');
        //#endregion
        
    })

    it('View a new note', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.get('[data-testid = note-view]').click()
        cy.wait(1000)
        //#endregion

        //#region Act
        // Change the date
        cy.get('#create-date-input').clear().type('2024-03-25')
        // Type into the note input field
        cy.get('#create-paragraph-input').click();
        cy.wait(1000)
        cy.get('#create-paragraph-input').type('Test View');
        // Click the submit button
        cy.get('#submit-note-button').click();
        // Sleep for sometime
        cy.wait(1000)
        // Select another date
        cy.get('#create-date-input').clear().type('2024-03-26')
        // Select the original date
        cy.get('#create-date-input').clear().type('2024-03-25')
        //#endregion

        //#region Assert
        // Check that the note content is displayed
        cy.get('#create-paragraph-input').should('contain', 'Test View');
        //#endregion
        
        // Clean up          
        cy.get('#note-delete-button').click()
        
    })

    it('Edit a new note', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)
        // Visit the page
        cy.get('[data-testid = note-view]').click()
        cy.wait(1000)
        //#endregion

        //#region Act
        // Change the date
        cy.get('#create-date-input').clear().type('2024-03-25')
        // Type into the note input field
        cy.get('#create-paragraph-input').click();
        cy.wait(1000)
        cy.get('#create-paragraph-input').type('Test Edit');
        // Click the submit button
        cy.get('#submit-note-button').click();
        // Sleep for sometime
        cy.wait(1000)
        // Select another date
        cy.get('#create-date-input').clear().type('2024-03-26')
        // Select the original date
        cy.get('#create-date-input').clear().type('2024-03-25')
        cy.get('#create-paragraph-input').click();
        cy.wait(1000)
        cy.get('#create-paragraph-input').type('ed');
        // Click the submit button
        cy.get('#submit-note-button').click();
        //#endregion

        //#region Assert
        // Check that the note content is displayed
        cy.get('#create-paragraph-input').should('contain', 'Test Edited');
        //#endregion
        
        // Clean up          
        cy.get('#note-delete-button').click()
        
    })

    
  });
describe('User Form Test', () => {
    let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638477345117710790","Iat":"638477333117710758","Nbf":null,"Scope":"","UserHash":"TestUser","Claims":{"Role":"Admin"}},"Signature":"lrs50BAplaJBayP3LEUCYscgQjjILbJrBVSOG4V9Jwc"}'
    it('Create a new user form on login', () => {
        //#region Arrange
        cy.visit('http://localhost:3000/');
        window.localStorage.clear()
        window.localStorage.setItem('token-local', token)

        cy.wait(2000)

        cy.get('#user-form-link').click()

        cy.wait(2000)

        function shuffleArray(array) {
            for (let i = array.length - 1; i > 0; i--) {
                const j = Math.floor(Math.random() * (i + 1));
                [array[i], array[j]] = [array[j], array[i]];
            }
            return array;
        }

        const ratings = shuffleArray([1, 2, 3, 4, 5, 6, 7, 8, 9, 10]);

        cy.get('#mental-health-rank').clear()
        cy.get('#mental-health-rank').type(ratings[0])

        cy.get('#physical-health-rank').clear()
        cy.get('#physical-health-rank').type(ratings[1])

        cy.get('#outdoor-rank').clear()
        cy.get('#outdoor-rank').type(ratings[2])

        cy.get('#sport-rank').clear()
        cy.get('#sport-rank').type(ratings[3])

        cy.get('#art-rank').clear()
        cy.get('#art-rank').type(ratings[4])

        cy.get('#hobby-rank').clear()
        cy.get('#hobby-rank').type(ratings[5])

        cy.get('#thrill-rank').clear()
        cy.get('#thrill-rank').type(ratings[6])

        cy.get('#travel-rank').clear()
        cy.get('#travel-rank').type(ratings[7])

        cy.get('#volunteering-rank').clear()
        cy.get('#volunteering-rank').type(ratings[8])

        cy.get('#food-rank').clear()
        cy.get('#food-rank').type(ratings[9])

        cy.wait(500)

        cy.get('#submit-ranking-button').click()

        cy.wait(1000)

        // Go to another page
        cy.get('#lli-view').click()

        cy.wait(1000)

        // Go back
        cy.get('#user-form-link').click()

        cy.wait(1000)

        cy.get('#mental-health-rank').should('have.value', ratings[0]);
        cy.get('#physical-health-rank').should('have.value', ratings[1]);
        cy.get('#outdoor-rank').should('have.value', ratings[2]);
        cy.get('#sport-rank').should('have.value', ratings[3]);
        cy.get('#art-rank').should('have.value', ratings[4]);
        cy.get('#hobby-rank').should('have.value', ratings[5]);
        cy.get('#thrill-rank').should('have.value', ratings[6]);
        cy.get('#travel-rank').should('have.value', ratings[7]);
        cy.get('#volunteering-rank').should('have.value', ratings[8]);
        cy.get('#food-rank').should('have.value', ratings[9]);
    })



});
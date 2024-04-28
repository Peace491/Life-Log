describe('Media Memento E2E Test', () => {
  let token = '{"Header":{"Alg":"HS256","Typ":"JWT"},"Payload":{"Iss":"localhost","Sub":"myApp","Aud":"myApp","Exp":"638498596492752550","Iat":"638498584492752350","Nbf":null,"Scope":"","UserHash":"System","Claims":{"Role":"Admin"}},"Signature":"7O-aDLgpvlNAHdhf2RBYfwyufue6q5_jkdE5b-l_Gd4"}'; // todo put in working token
  it('Upload Media', () => {
    // Arrange
    cy.visit('http://localhost:3000/');
    window.localStorage.clear()
    window.localStorage.setItem('token-local', token)
    // Visit the page
    cy.wait(500)

    // Act
    cy.get('div#93.lli').click();
    // cy.get('div.lli-media-container').click();
    cy.wait(100)

    // Assert
    cy.get('div.lli-media-container').should('contain', 'img');
  })
  it('View Media', () => {
    // Arrange
    cy.visit('http://localhost:3000/');
    window.localStorage.clear()
    window.localStorage.setItem('token-local', token)
    // Visit the page
    cy.wait(500)

    // Act
    cy.get('div#93.lli').click();
    // cy.get('div.lli-media-container').click();
    cy.wait(500)

    // Assert
  })
  it('Update Media', () => {
    // Arrange
    cy.visit('https://example.cypress.io')

    // Act

    // Assert
  })
  it('Delete Media', () => {
    // Arrange
    cy.visit('https://example.cypress.io')

    // Act

    // Assert
  })
})
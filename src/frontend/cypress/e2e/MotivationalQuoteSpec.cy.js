describe('Motivational Quote Test', () => {
  it('Check Quote is there', () => {
    cy.visit("http://localhost:3000/");

    cy.get('[data-testid = "quote-title"]').should('exist')
    .should('not.be.empty');
  })

  it('Check Author is there', () => {
    cy.visit("http://localhost:3000/");

    cy.get('[data-testid = "author-title"]').should('exist')
    .should('not.be.empty');
  })

  it('Check Placeholder is there', () => {
    cy.visit("http://localhost:3000/");

    cy.get('[data-testid = "quote-title"]').should('exist')
    .should('not.have.text', 'Motivational Quotes will be right back!');
    cy.get('[data-testid = "author-title"]').should('exist')
    .should('not.have.text', 'Team Peace');
  })
})
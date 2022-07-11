import React from 'react';
import { render, screen } from '@testing-library/react';
import App from './App';

test('renders stuff', () => {
  render(<App />);
  const homeLinkAndJumbotron = screen.getAllByText(/project omega/i)
  expect(homeLinkAndJumbotron).toHaveLength(2)
});

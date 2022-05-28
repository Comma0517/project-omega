import React from 'react';
import logo from './logo.svg';
import './App.css';
import axios from 'axios'

function App() {
  async function apiTest() {
    try {
      let response = await axios.get('/api/WeatherForecast/omega')
      console.log(response)
    } catch (err) {
      console.log('Error accessing api: ', err)
    }
  }
  
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>Edit <code>src/App.tsx</code> and save to reload.</p>
        <a className="App-link" href="https://reactjs.org" target="_blank" rel="noopener noreferrer">Learn React</a>
        <p><button onClick={apiTest}>API Test</button> (see console)</p>
      </header>
    </div>
  );
}

export default App;

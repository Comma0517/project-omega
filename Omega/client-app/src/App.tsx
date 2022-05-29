import './App.css';
import axios from 'axios'

function App() {
  async function apiTest(weatherEndpoint: string = '') {
    console.log('attempting to hit api...')
    try {
      let response = await axios.get(`/api/WeatherForecast/${weatherEndpoint}`)
      console.log(response)
    } catch (err) {
      console.log('Error accessing api: ', err)
    }
  }
  
  return (
    <div className="App">
      <header className="App-header">
        <p><button onClick={() => apiTest()}>Weather API Test</button> (see console)</p>
        <p><button onClick={() => apiTest('omega')}>Omega API Test</button> (see console)</p>
      </header>
    </div>
  );
}

export default App;

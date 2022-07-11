import './App.css'
import Nav from './components/nav'
import {
  BrowserRouter as Router,
  Switch,
  Route
} from 'react-router-dom'
import {NotFound} from './components/NotFound';
import {Home} from './components/Home';

function App() {
  return (
    <Router>
      <Nav/>
      <main role="main" className="container" style={{paddingTop: 80}}>
        <Switch>
          <Route path="/" exact={true}>
            <Home/>
          </Route>
          <Route path="*">
            <NotFound/>
          </Route>
        </Switch>
      </main>
    </Router>
  )
}

export default App

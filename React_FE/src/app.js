import React from 'react'
import {BrowserRouter as Router, Route, Switch} from 'react-router-dom'
import NavigationBar from './navigation-bar'
//import Home from './home/home';
import Auth from './auth/auth'
import User from './user/user-container'
import ErrorPage from './commons/errorhandling/error-page';
import styles from './commons/styles/project-style.css';
import DeviceContainer from './device/device-container';
class App extends React.Component {


    render() {

        return (
            <div className={styles.back}>
            <Router>
                <div>
                    <NavigationBar />
                    <Switch>

                        <Route
                            exact
                            path='/'
                            render={() => <Auth/>}
                        />
                        <Route
                            exact
                            path='/admin'
                            render={() => <User/>}
                        />
                        <Route 
                            path="/devices/:userId" component={DeviceContainer} />
                        

                        {/*Error*/}
                        <Route
                            exact
                            path='/error'
                            render={() => <ErrorPage/>}
                        />

                        <Route render={() =><ErrorPage/>} />
                    </Switch>
                </div>
            </Router>
            </div>
        )
    };
}

export default App

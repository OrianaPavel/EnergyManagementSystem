import React from 'react';

import BackgroundImg from '../commons/images/future-medicine.jpg';

import {Button, Container, Jumbotron} from 'reactstrap';

const backgroundStyle = {
    backgroundPosition: 'center',
    backgroundSize: 'cover',
    backgroundRepeat: 'no-repeat',
    width: "100%",
    height: "1920px",
    backgroundImage: `url(${BackgroundImg})`
};
const textStyle = {color: 'white', };

class Home extends React.Component {


    render() {

        return (

            <div>
                <Jumbotron fluid style={backgroundStyle}>
                    <Container fluid>
                        <h1 className="display-3" style={textStyle}></h1>
                        <p className="lead" style={textStyle}> <b></b> </p>
                        <hr className="my-2"/>
                        <p  style={textStyle}> <b>Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor 
                            incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco 
                            laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit
                             esse cillum dolore eu fugiat nulla pariatur.
                             Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum. </b> </p>
                        <p className="lead">
                            <Button color="primary" onClick={() => window.open('http://www.google.com')}>Learn
                                More</Button>
                        </p>
                    </Container>
                </Jumbotron>

            </div>
        )
    };
}

export default Home

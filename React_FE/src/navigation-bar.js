import React from 'react'
import logo from './commons/images/icon.png';

import {
    DropdownItem,
    DropdownMenu,
    DropdownToggle,
    Nav,
    Navbar,
    NavbarBrand,
    NavLink,
    UncontrolledDropdown
} from 'reactstrap';

const textStyle = {
    color: 'white',
    textDecoration: 'none'
};

/*
    <DropdownItem>
        <NavLink href="/person">Persons</NavLink>
    </DropdownItem>
*/

function handleLogOut(event) {
    localStorage.clear();
}

const NavigationBar = () => (
    <div>
        <Navbar color="dark" light expand="md">
            <NavbarBrand href="/">
                <img src={logo} width={"50"}
                    height={"35"} />
            </NavbarBrand>
            <Nav className="mr-auto" navbar>

                <UncontrolledDropdown nav inNavbar>
                    <DropdownToggle style={textStyle} nav caret>
                        Menu
                    </DropdownToggle>
                    <DropdownMenu right >

                        <DropdownItem>
                            <NavLink href="/" onClick={handleLogOut}>LogOut</NavLink>

                        </DropdownItem>
                        <DropdownItem>
                            {localStorage.getItem('isLoggedIn') && (
                                <NavLink href="/chat">Chat</NavLink>
                            )}
                        </DropdownItem>

                    </DropdownMenu>
                </UncontrolledDropdown>

            </Nav>
        </Navbar>
    </div>
);

export default NavigationBar

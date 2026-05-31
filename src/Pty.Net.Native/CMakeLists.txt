cmake_minimum_required(VERSION 3.10)
project(pty_net C)

set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED ON)

if(NOT CMAKE_BUILD_TYPE)
    set(CMAKE_BUILD_TYPE Release)
endif()

set(CMAKE_C_FLAGS "${CMAKE_C_FLAGS} -Wall -Wextra")
set(CMAKE_C_FLAGS_RELEASE "${CMAKE_C_FLAGS_RELEASE} -O2")
set(CMAKE_C_VISIBILITY_PRESET hidden)

add_library(pty_net SHARED pty_shim.c)

if(APPLE)
    target_link_libraries(pty_net PRIVATE "-framework CoreFoundation")
    set_target_properties(pty_net PROPERTIES
        INSTALL_RPATH "@loader_path"
        BUILD_WITH_INSTALL_RPATH TRUE
        MACOSX_RPATH TRUE
        SUFFIX ".dylib"
        PREFIX "lib"
    )
else()
    target_link_libraries(pty_net PRIVATE util)
    set_target_properties(pty_net PROPERTIES
        SUFFIX ".so"
        PREFIX "lib"
    )
endif()

set_target_properties(pty_net PROPERTIES
    LIBRARY_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
    RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
)
